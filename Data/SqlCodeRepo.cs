using Comander.Models;
using Commander.Data;
using Commander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Data
{
    public class SqlCodeRepo : ICodeRepo
    {
        private readonly CommanderContext _context;

        public SqlCodeRepo(CommanderContext context)
        {
            _context = context;
        }

        public void AddCode(CodeModel code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            _context.Code.Add(code);

        }

        public void DeactiveCode(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            try
            {
                var x = _context.Code.First(p => p.UserId == user.Id);
            }catch(Exception e)
            {
            }
            

            
        }

        public CodeModel GetCodeModelByCode(string code)
        {
            CodeModel codeModel = _context.Code.FirstOrDefault(p => p.Code == code);

            return codeModel;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

    }
}
