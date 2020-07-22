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
        public ActionResult getCode(string code)
        {
            CodeModel codeModel = _repositoryCodes.GetCodeModelByCode(code);
            bool codeIsValid = CodeHandler.IsCodeValid(codeModel);
            if (codeIsValid)
            {
                UserModel userModel = _repositoryUsers.GetUserById(codeModel.UserId);
                
            }
            var x = 5;
            return Ok();
        }

        [HttpPost]
        public IActionResult PostLogin(UserDto userDto)
        {
            UserModel login = new UserModel();
            var loginDatas = _mapper.Map<UserModel>(userDto);
            login.UserLogin = loginDatas.UserLogin;
            login.UserPass = loginDatas.UserPass;
            login.Id = loginDatas.Id;
            login.UserRole = "user";

            IActionResult response = Unauthorized();

            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenStr = GenerateJSOWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;
        }

        [HttpPost]
        public IActionResult GetUsersData(UserToken token)
        {
            //UserModel userLoginInModel = _mapper.Map<UserModel>(userDto);
            var jwt = token.tokenCode;
            var handler = new JwtSecurityTokenHandler();
            var tokenDecoded = handler.ReadJwtToken(jwt);
            UserModel userModelTokenOnly = new UserModel();
            userModelTokenOnly.UserLogin = tokenDecoded.Subject;
            UserModel userModel = _repositoryUsers.GetUserByLogin(userModelTokenOnly.UserLogin);
            userModel.UserPass = null;
            userModel.UserSalt = null;
            return Ok(_mapper.Map<UserDto>(userModel));

            //return Ok();
        }

        private UserModel AuthenticateUser(UserModel login)
        {
            UserModel user = null;
            var DbUser = _repositoryUsers.GetUserByLogin(login.UserLogin);
            var saltAsString = DbUser.UserSalt;
            var saltAsByte = Encoding.UTF8.GetBytes(saltAsString);
            login.UserPass = HashPassword(saltAsByte, login.UserPass);
            if (login.UserLogin.ToUpper() == DbUser.UserLogin.ToUpper() && login.UserPass == DbUser.UserPass) {
                user = new UserModel {
                    UserLogin = DbUser.UserLogin,
                    UserMail = DbUser.UserMail,
                    Id = DbUser.Id,
                    UserRole = DbUser.UserRole };
            }

            return user;
        }

        private string GenerateJSOWebToken(UserModel userInfo) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creditentals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserLogin),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.UserMail),
                //new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", userInfo.UserRole)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(1),
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
            userDto.UserRole = "user";
            var commonModel = _mapper.Map<UserModel>(userDto);
            if (!isLoginUnique(commonModel))
            {
                return NotFound("Login is not unique");
            }
            if (!isEmailUnique(commonModel))
            {
                return NotFound("e-mail is not unique");
            }

            var saltAsByte = GetSalt();
            var saltAsString = Encoding.UTF8.GetString(saltAsByte, 0, saltAsByte.Length);

            commonModel.Id = Guid.NewGuid().ToString();
            commonModel.UserSalt = saltAsString;
            commonModel.UserPass = HashPassword(saltAsByte, commonModel.UserPass);
            commonModel.Confirmed = false;
            _repositoryUsers.Register(commonModel);
            _repositoryUsers.SaveChanges();
            string userId = commonModel.Id;

            string rawCode = CodeHandler.GenerateRawCode();
            DateTime cretionDate = DateTime.UtcNow;
            DateTime expireDate = cretionDate.AddDays(3);
            TypeOfCode typeOfCode = TypeOfCode.RegistrationCode;
            CodeModel codeModel  = CodeHandler.CodeModelCreator(rawCode, commonModel, cretionDate, expireDate, typeOfCode);
            EmailSender email = new EmailSender();

            _repositoryCodes.DeactiveCode(commonModel);
            email.VeryfiEmail(commonModel.UserMail, rawCode);
            var code = _mapper.Map<CodeModel>(codeModel);
            _repositoryCodes.AddCode(code);
            _repositoryCodes.SaveChanges();

            return Ok(commonModel);
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

        private bool isLoginUnique(UserModel user)
        {
            return !_repositoryUsers.isUserInDb(user);
        }

        private bool isEmailUnique(UserModel user)
        {
            return !_repositoryUsers.isEmailInDb(user);
        }

    }

    public class UserToken
    {
         public string tokenCode { get; set; }
    } 
}

