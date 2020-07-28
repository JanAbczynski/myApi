using Comander.Models;
using Commander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Controllers
{
    public static class CodeHandler
    {
        static string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        public static CodeModel CodeModelCreator(string rawCode, UserModel userModel, DateTime creationDate, DateTime expireDate, MailType typeOfCode)
        {
            CodeModel userCode = new CodeModel();
            userCode.Idc = Guid.NewGuid().ToString();
            userCode.Code = rawCode;
            userCode.UserId = userModel.Id;
            userCode.UserLogin = userModel.UserLogin;
            userCode.CreationTime = creationDate;
            userCode.ExpireTime = expireDate;
            userCode.TypeOfCode = typeOfCode.ToString();
            userCode.IsActive = true;

            return userCode;
        }

        public static DateTime SetExpireDate(DateTime cretionDate, MailType mailType)
        {
            DateTime expireDate = cretionDate.AddDays(1);
            switch (mailType)
            {
                case MailType.varyfication:
                    expireDate = cretionDate.AddDays(3);
                    break;
                case MailType.recovery:
                    expireDate = cretionDate.AddMinutes(30);
                    break;
            }
            return expireDate;
        }

        public static string GenerateRawCode(int? additionalLength = 0)
        {
            Random random = new Random(characters.Length);
            string additionalCode = "X";
            for(int i = 0; i < additionalLength; i++)
            {
                
            }
            Guid validCode = Guid.NewGuid();
            return validCode.ToString() + additionalCode;
        }

        public static bool IsCodeValid(CodeModel codeModel, MailType type)
        {
            DateTime now = DateTime.UtcNow;
            if (codeModel.WasUsed || !codeModel.IsActive)
            {
                return false;
            }

            if (codeModel.ExpireTime <= now)
            {
                return false;
            }

            if (codeModel.TypeOfCode != type.ToString())
            {
                return false;
            }




            return true;
        }
    }
}

public enum TypeOfCode
{
    RegistrationCode,
    ChangePasswordCode
}
