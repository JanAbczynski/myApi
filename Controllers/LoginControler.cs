using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Comander.Dtos;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using Comander.Models;
using Comander.Data;
using System.Data;
using Newtonsoft.Json;
using System.Linq;

namespace Comander.Controllers
{
    //api/commands
    // [EnableCors("_Policy")]
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ICommanderRepo _repository;
        private readonly IUserRepo _repositoryUsers;
        private readonly ICodeRepo _repositoryCodes;
        private readonly IMapper _mapper;
        private IConfiguration _config;
        SmtpClient cv = new SmtpClient("smtp.gmail.com", 587);
        static string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";

        public LoginController(IUserRepo repositoryUsers, ICodeRepo repositoryCodes, IMapper mapper, IConfiguration config)
        {
            _repositoryUsers = repositoryUsers;
            _repositoryCodes = repositoryCodes;

            _mapper = mapper;
            _config = config;
        }



        [HttpGet]
        public ActionResult ValidateUser(string code)
        {

            string sqlProc = "exec dbo.ValidateUser";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@Code", code}
            };
            DbHandler dbHandler = new DbHandler();
            sqlProc = dbHandler.AddParamsToQuery(sqlProc, queryParams);
            try
            {
                DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);
                return Ok(new { Result = "Confirmation ok" });
            }
            catch(Exception e)
            {
                return Conflict(new { Result = "Code is not good" });
            }
            




            //CodeModel codeModel = _repositoryCodes.GetCodeModelByCode(code);
            //bool codeIsValid = CodeHandler.IsCodeValid(codeModel, MailType.varyfication);
            //if (codeIsValid)
            //{
            //    UserModel userModel = _repositoryUsers.GetUserById(codeModel.UserId);
            //    UserModel userModelToSave = new UserModel();
            //    userModel.Confirmed = true;
            //    userModelToSave = userModel;
            //    ChangeUserData(userModelToSave, userModel);

            //    CodeModel usedCode = _repositoryCodes.GetCodeModelByCode(code);
            //    CodeModel CodeModelToSave = CodeHandler.DeactivateCode(usedCode, userModel);

            //    ChangeCodeData(CodeModelToSave, usedCode);
            //}

        }


        //[HttpPost]
        //public ActionResult TryToChangePass(CodeModel codeModel)      
        //{
        //    CodeModel recoveryCode = _repositoryCodes.GetCodeModelByCode(codeModel.Code);

        //    if (!CodeHandler.IsCodeValid(recoveryCode, MailType.recovery))
        //    {
        //        return Unauthorized();
        //    }

        //    UserModel user = _repositoryUsers.GetUserById(recoveryCode.UserId);
        //    string rawCode = CodeHandler.GenerateRawCode(5);
        //    DateTime creationDate = DateTime.UtcNow;
        //    DateTime expireDate = creationDate.AddMinutes(10);
        //    CodeModel changerCode = CodeHandler.CodeModelCreator(rawCode, user, creationDate, expireDate, MailType.changePassword);
        //    _repositoryCodes.AddCode(changerCode);
        //    _repositoryCodes.SaveChanges();

        //    CodeModel usedCode = CodeHandler.DeactivateCode(recoveryCode, user);
        //    ChangeCodeData(usedCode, recoveryCode);
            
        //    return Ok(new { status = true, codeForChange = rawCode});
        //}


        private void ChangeUserData(UserModel newUserData, UserModel oldUserData)
        {
                _mapper.Map(newUserData, oldUserData);
                _repositoryUsers.SaveChanges();
        }

        //private void ChangeCodeData(CodeModel newCodeData, CodeModel oldCodeData)
        //{
        //    _mapper.Map(newCodeData, oldCodeData);
        //    _repositoryUsers.SaveChanges();
        //}

        [HttpPost]
        public IActionResult PostLogin(UserDto userDto)
        {
            UserModel login = new UserModel();
            var loginDatas = _mapper.Map<UserModel>(userDto);
            login.UserLogin = loginDatas.UserLogin;
            login.UserPass = loginDatas.UserPass;
            UserModel user = null;

            IActionResult response = Unauthorized();
            try
            {
                user = AuthenticateUser(login);
            }catch(Exception e)
            {
                return response;
            }
            
            if (user != null)
            {
                string tokenStr = GenerateJSOWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;
        }

        [HttpPost]
        public ActionResult RemindPassword(UserModel userModel)
        {
            string code = "";
            string sqlProc = "exec dbo.StartRecoverPass";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@UserMail", userModel.UserMail}
            };
            DbHandler dbHandler = new DbHandler();
            sqlProc = dbHandler.AddParamsToQuery(sqlProc, queryParams);
            try
            {
                DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);
                foreach (DataRow row in dataSet.Tables["tab"].Rows)
                {
                    code = row["Code"].ToString();
                    MailOperator(userModel, MailType.recovery, code);
                }
            }
            catch (Exception e)
            {

            }



            return Ok();
        }

        [Authorize]
        [HttpPost]
        public ActionResult PassChangerFromPanel(TempModel passedTempModel)
        {
            string tempNewPass = passedTempModel.userPass;
            UserModel newUserModel = null;
            //UserModel oldUserModel = GetUserInfoFromToken(passedTempModel.token);
            UserModel oldUserModel = UserHandler.GetUserDataByToken(passedTempModel.token);
            oldUserModel.UserPass = passedTempModel.oldPass;
            //oldUserModel = GetUserDataFromDbByLogin(oldUserModel.UserLogin);
            oldUserModel = UserHandler.GetUserDataFromDbByLogin(oldUserModel.UserLogin);
            oldUserModel.UserPass = passedTempModel.oldPass;
            newUserModel = AuthenticateUser(oldUserModel);

            if (newUserModel == null)
            {
                return NotFound();
            }

            var saltAsByte = GetSalt();

            var saltAsString = Encoding.UTF8.GetString(saltAsByte, 0, saltAsByte.Length);
            var hashedPassword = HashPassword(saltAsByte, passedTempModel.userPass);

            newUserModel.UserPass = hashedPassword;
            newUserModel.UserSalt = saltAsString;
            UpdateUserDataInDB(newUserModel);

            //dbHandler.GenerateProcedure(sqlProc, queryParams);
            return Ok();
        }

        private void UpdateUserDataInDB(UserModel newUserModel)
        {
            string sqlProc = "exec UpdateUserData";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@Id",newUserModel.Id},
                      { "@UserType",newUserModel.UserType},
                      { "@UserLogin",newUserModel.UserLogin},
                      { "@UserPass",newUserModel.UserPass},
                      { "@UserName",newUserModel.UserName},
                      { "@UserSureName",newUserModel.UserSureName},
                      { "@UserTaxNumber",newUserModel.UserTaxNumber},
                      { "@UserAddress",newUserModel.UserAddress},
                      { "@UserCity",newUserModel.UserCity},
                      { "@UserZipCode",newUserModel.UserZipCode},
                      { "@UserMail",newUserModel.UserMail},
                      { "@UserPhoneNumber",newUserModel.UserPhoneNumber},
                      { "@UserPhoneNumber2",newUserModel.UserPhoneNumber2},
                      { "@UserSalt",newUserModel.UserSalt},
                      { "@UserRole",newUserModel.UserRole},
                      { "@Confirmed", newUserModel.Confirmed}
            };

            DbHandler dbHandler = new DbHandler();
            dbHandler.GenerateProcedure(sqlProc, queryParams);

            sqlProc = dbHandler.AddParamsToQuery(sqlProc, queryParams);
            DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);

            throw new NotImplementedException();
        }

        private UserModel GetUserDataFromDbByLogin(string login)
        {
            UserModel userModel = new UserModel();
            string sqlProc = "SELECT * FROM Users WHERE UserLogin = @UserLogin";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@UserLogin", login }
            };
            DbHandler dbHandler = new DbHandler();
            try
            {
                DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);
                foreach (DataRow row in dataSet.Tables["tab"].Rows)
                {

                    userModel.Id = row["Id"].ToString();
                    userModel.UserType = row["UserType"].ToString();
                    userModel.UserLogin = row["UserLogin"].ToString();
                    userModel.UserPass = row["UserPass"].ToString();
                    userModel.UserName = row["UserName"].ToString();
                    userModel.UserSureName = row["UserSureName"].ToString();
                    userModel.UserTaxNumber = row["UserTaxNumber"].ToString();
                    userModel.UserAddress = row["UserAddress"].ToString();
                    userModel.UserCity = row["UserCity"].ToString();
                    userModel.UserZipCode = row["UserZipCode"].ToString();
                    userModel.UserMail = row["UserMail"].ToString();
                    userModel.UserPhoneNumber = row["UserPhoneNumber"].ToString();
                    userModel.UserPhoneNumber2 = row["UserPhoneNumber2"].ToString();
                    userModel.UserSalt = row["UserSalt"].ToString();
                    userModel.UserRole = row["UserRole"].ToString();
                    userModel.Confirmed = Convert.ToBoolean(row["Confirmed"]);
                }
            }catch(Exception e)
            {

            }
            return userModel;
        }

        [HttpPost]
        public ActionResult PassChanger(TempModel passedModel)
        {
            var saltAsByte = GetSalt();
            var saltAsString = Encoding.UTF8.GetString(saltAsByte, 0, saltAsByte.Length);
            var hashedPassword = HashPassword(saltAsByte, passedModel.userPass);

            string sqlProc = "exec dbo.ChangeUserPassword";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@NewPassword",hashedPassword},
                      { "@Code",passedModel.code },
                      { "@Salt",saltAsString }
            };
            DbHandler dbHandler = new DbHandler();
            sqlProc = dbHandler.AddParamsToQuery(sqlProc, queryParams);
            try
            {
                UserModel userModel = new UserModel();
                DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);
                foreach (DataRow row in dataSet.Tables["tab"].Rows)
                {
                    userModel.UserMail = row["UserMail"].ToString();
                    userModel.UserLogin = row["UserLogin"].ToString();

                    MailOperator(userModel, MailType.changePassConfirmation);
                }
                    
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult PassChanger2 (TempModel passedModel)
        {

            var jwtToken = new JwtSecurityToken(passedModel.token);
            UserModel userToCheck = new UserModel();
            userToCheck.UserPass = passedModel.oldPass;
            userToCheck.UserLogin = jwtToken.Subject;

            UserModel user = AuthenticateUser(userToCheck);
            if (user != null)
            { 
                FinallyChangePass(user, passedModel.userPass);
                return Ok();
            }

            return Unauthorized();
        }

        private ActionResult FinallyChangePass(UserModel userWithOldPass, string newPass)
        {
            UserModel userWithNewPass = userWithOldPass;
            var saltAsByte = GetSalt();
            var saltAsString = Encoding.UTF8.GetString(saltAsByte, 0, saltAsByte.Length);
            userWithNewPass.UserSalt = saltAsString;
            userWithNewPass.UserPass = HashPassword(saltAsByte, newPass);
            ChangeUserData(userWithNewPass, userWithOldPass);

            return Ok();
        }

        public UserModel GetUserInfoFromToken(string code)
        {
            UserModel userModel = new UserModel();
            var jwt = code;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDecoded = tokenHandler.ReadJwtToken(jwt);
            userModel.UserLogin = tokenDecoded.Subject;
            userModel.UserMail = tokenDecoded.Claims.FirstOrDefault(x => x.Type == "email").Value;
            userModel.UserType = tokenDecoded.Claims.FirstOrDefault(x => x.Type == "userType").Value;
            userModel.UserRole = tokenDecoded.Claims.FirstOrDefault(x => x.Type == "role").Value;

            return userModel;
        }

        //[HttpPost]
        //public IActionResult GetUsersData(UserToken token)
        //{
        //    var jwt = token.tokenCode;
        //    var handler = new JwtSecurityTokenHandler();
        //    var tokenDecoded = handler.ReadJwtToken(jwt);
        //    UserModel userModelTokenOnly = new UserModel();
        //    userModelTokenOnly.UserLogin = tokenDecoded.Subject;
        //    UserModel userModel = _repositoryUsers.GetUserByLogin(userModelTokenOnly.UserLogin);
        //    userModel.UserPass = null;
        //    userModel.UserSalt = null;

        //    return Ok(_mapper.Map<UserDto>(userModel));
        //}

        private UserModel AuthenticateUser(UserModel loginUserModel)
        {
            UserModel userLoginDataFromDb = null;
            userLoginDataFromDb = UserHandler.GetUserDataFromDbByLogin(loginUserModel.UserLogin);
            //userLoginDataFromDb = GetUserDataFromDbByLogin(loginUserModel.UserLogin);
            var saltAsByte = Encoding.UTF8.GetBytes(userLoginDataFromDb.UserSalt);
            loginUserModel.UserPass = HashPassword(saltAsByte, loginUserModel.UserPass);
            if (loginUserModel.UserPass == userLoginDataFromDb.UserPass)
            {          
                return userLoginDataFromDb;
            }
            else
            {
                userLoginDataFromDb = null;
                return userLoginDataFromDb;
            }
        }

        private string GenerateJSOWebToken(UserModel userInfo) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creditentals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserLogin),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.UserMail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userType", userInfo.UserType),
                new Claim("role", userInfo.UserRole)
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creditentals);
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        [Authorize]
        [HttpPost]
        public string Post() {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            return "Welcome To: " + userName;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAction()
        {
            return new string[] { "val; 1", "val 2", "val 3" };
        }

        [HttpPost]
        public ActionResult<UserModel> RegisterNewUser(UserDto userDto)
        {
            var saltAsByte = GetSalt();
            var saltAsString = Encoding.UTF8.GetString(saltAsByte, 0, saltAsByte.Length);
            var hashedPassword = HashPassword(saltAsByte, userDto.UserPass);
            string code = "";

            string sqlProc = "exec dbo.RegisterUser";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@UserType",userDto.UserType},
                      { "@UserLogin",userDto.UserLogin },
                      { "@UserPass",hashedPassword},
                      { "@UserName",userDto.UserName},
                      { "@UserSureName",userDto.UserSureName},
                      { "@UserTaxNumber",userDto.UserTaxNumber},
                      { "@UserAddress",userDto.UserAddress},
                      { "@UserCity",userDto.UserCity},
                      { "@UserZipCode",userDto.UserZipCode},
                      { "@UserMail",userDto.UserMail},
                      { "@UserPhoneNumber",userDto.UserPhoneNumber},
                      { "@UserPhoneNumber2",userDto.UserPhoneNumber2},
                      { "@UserSalt",saltAsString},
                      { "@UserRole","user"},
                      { "@Confirmed", false.ToString()}
            };
            UserModel userModel = new UserModel();
            DbHandler dbHandler = new DbHandler();
            sqlProc = dbHandler.AddParamsToQuery(sqlProc, queryParams);
            try
            {
                DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);
                string JSONModel = string.Empty;
                foreach (DataRow row in dataSet.Tables["tab"].Rows)
                {
                   JSONModel = JsonConvert.SerializeObject(row.Table);
                    userModel = JsonConvert.DeserializeObject<UserModel[]>(JSONModel).FirstOrDefault();
                }
                foreach (DataRow row in dataSet.Tables["tab1"].Rows)
                {
                    code = row["Code"].ToString();
                    string typeOfCode = row["TypeOfCode"].ToString();
                }
            }catch(Exception e)
            {
                return Conflict(e.Message);
            }
            MailOperator(userModel, MailType.varyfication, code);

            return Ok(userModel);
        }

        [HttpGet("{code}", Name = "ValidateCode")]
        public ActionResult<UserModel> ValidateCode(string code)
        {
            Console.WriteLine("sss");
            return Ok();
        }

        private bool MailOperator(UserModel userReciever, MailType mailType, string code = null)
        {
            EmailSender email = new EmailSender();
            string mailBody = email.BodyBuilder(code, mailType, userReciever);
            string mailSubject = email.CreateSubject(mailType);
            email.PrepareEmail(userReciever.UserMail, mailBody, mailSubject);
            return true;
        }
    
        private string HashPassword(byte[] salt, string password)
        {
            byte[] passAsByte = Encoding.ASCII.GetBytes(password);
            var hashedPassByte = ComputeHMAC_SHA256(passAsByte, salt);
            string hashedPassString = Encoding.UTF8.GetString(hashedPassByte, 0, hashedPassByte.Length);
            return hashedPassString;
        }

        private byte[] GetSalt()
        {
            var salt = GenerateSalt();
            return salt;
        }

        public static byte[] ComputeHMAC_SHA256(byte[] data, byte[] salt)
        {
            using (var hmac = new HMACSHA256(salt))
            {
                return hmac.ComputeHash(data);
            }
        }

        public static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[5];
                rng.GetBytes(randomNumber);
                var temp_string = Encoding.UTF8.GetString(randomNumber, 0, randomNumber.Length);
                var randomNumberUTF8 = Encoding.UTF8.GetBytes(temp_string);
                return randomNumberUTF8;
            }
        }

    }

    public class UserToken
    {
        public string tokenCode { get; set; }
        public string Subject { get; set; }
    } 

    public enum UserType
    {
        person,
        company
    }
    public enum MailType
    {
        varyfication,
        recovery,
        changePassConfirmation
    }
}

