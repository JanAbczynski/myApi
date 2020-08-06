using Comander.Models;
using Commander.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Data
{
    public class SqlRunRepo : IRunRepo
    {
        private readonly CommanderContext _context;

        public SqlRunRepo (CommanderContext context)
        {
            _context = context;
        }

        public IEnumerable<RunModel> GetAllRuns()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RunModel> GetRunByCompetitionId(string competitionId)
        {
            return _context.Run.Where(p => p.competitionId == competitionId);

        }

        public void Register(RunModel run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }
            _context.Run.Add(run);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
