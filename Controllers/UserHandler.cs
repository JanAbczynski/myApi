using Commander.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Controllers
{
    public static class UserHandler
    {
        static public UserModel GetUserDataByToken(string token)
        {
            UserModel userModel = new UserModel();
            var jwt = token.Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDecoded = tokenHandler.ReadJwtToken(jwt);
            userModel.UserLogin = tokenDecoded.Subject;
            userModel.UserMail = tokenDecoded.Claims.FirstOrDefault(x => x.Type == "email").Value;
            userModel.UserType = tokenDecoded.Claims.FirstOrDefault(x => x.Type == "userType").Value;
            userModel.UserRole = tokenDecoded.Claims.FirstOrDefault(x => x.Type == "role").Value;
           
            return userModel;
        }

        static public UserModel GetUserDataFromDbByLogin(string login)
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
            }
            catch (Exception e)
            {
            }
            return userModel;
        }



    }
}
