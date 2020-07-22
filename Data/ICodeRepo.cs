using Comander.Models;
using Commander.Models;
using System.Collections.Generic;




namespace Commander.Data
{
    public interface ICodeRepo
    {
        public void AddCode(CodeModel code);
        public bool SaveChanges();
        public void DeactiveCode(UserModel user);
        public CodeModel GetCodeModelByCode(string code);
    }
}