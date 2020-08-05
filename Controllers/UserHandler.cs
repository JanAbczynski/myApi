using Commander.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Controllers
{
    public static class UserHandler
    {
        static public UserModel GetUserLoginByToken(string token)
        {
            string rawToken = token.Replace("Bearer ", "");
            var jwtToken = new JwtSecurityToken(rawToken);
            UserModel user = new UserModel();
            user.UserLogin = jwtToken.Subject;
           
            return user;
        }
    }
}
