using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Comander.Dtos;
using Comander.Models;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Comander.Controllers
{
    //api/commands
    // [EnableCors("_Policy")]
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class LoginController: ControllerBase
    {
        private readonly ICommanderRepo _repository;
        private readonly IMapper _mapper;
        private IConfiguration _config;


        public LoginController(ICommanderRepo repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
        }

        // POST api/login/postlogin

        [HttpPost]
        //public ActionResult<UserDto> PostLogin(string username, string password)
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
        // GET api/login/login
        [HttpGet]
        public IActionResult Login(string username, string password)
        {
            UserModel login = new UserModel();
            login.UserLogin = username;
            login.UserPass = password;
            login.Id = 6969;
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

        private UserModel AuthenticateUser(UserModel login)
        {
            UserModel user = null;
            if(login.UserLogin == "John" && login.UserPass == "qwe123"){
                user = new UserModel {
                    UserLogin = "John", 
                    UserPass ="qwe123", 
                    UserMail = "1@3.4", 
                    Id = login.Id,
                    UserRole = login.UserRole};
            }

            return user;
        }

        private string GenerateJSOWebToken(UserModel userInfo){
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

        //GET api/commands/post
        [Authorize]
        [HttpPost]
        public string Post(){
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            return "Welcome To: " + userName;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAction()
        {
            return new string[] { "val; 1", "val 2", "val 3"};
        }
    }
}