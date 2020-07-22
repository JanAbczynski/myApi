using Comander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Controllers
{
    public static class CodeHandler
    {
        public static CodeModel CodeModelCreator(string rawCode, string userLogin, DateTime creationDate, DateTime expireDate)
        {
            CodeModel userCode = new CodeModel();
            userCode.Code = rawCode;
            userCode.UserLogin = userLogin;
            userCode.CreationTime = creationDate;
            userCode.ExpireTime = expireDate;
            return userCode;
        }

        public static string GenerateRawCode()
        {
            Guid validCode = Guid.NewGuid();
            return validCode.ToString();
        }
    }
}
