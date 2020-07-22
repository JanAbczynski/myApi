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


        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

    }
}
