using Comander.Models;
using Commander.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Data
{
    public class SqlCompetitionRepo : ICompetitionRepo
    {
        private readonly CommanderContext _context;

        public SqlCompetitionRepo(CommanderContext context)
        {
            _context = context;
        }

        public void CompetitionUpdate(CompetitionModel competition)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CompetitionModel> GetAllCompetition()
        {
            return _context.Competition.ToList();
        }

        public IEnumerable<CompetitionModel> GetCompetition()
        {
            throw new NotImplementedException();
        }

        public CompetitionModel GetCompetitionById(string id)
        {
            return _context.Competition.FirstOrDefault(p => p.Id == id);
        }

        public bool isCompetitionInDb(CompetitionModel competition)
        {
            throw new NotImplementedException();
        }

        public void Register(CompetitionModel competition)
        {
            if (competition == null)
            {
                throw new ArgumentNullException(nameof(competition));
            }

            _context.Competition.Add(competition);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
