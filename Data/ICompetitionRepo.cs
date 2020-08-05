using Comander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Data
{
    public interface ICompetitionRepo
    {
        bool SaveChanges();
        IEnumerable<CompetitionModel> GetCompetition();
        IEnumerable<CompetitionModel> GetAllCompetition();
        IEnumerable<CompetitionModel> GetAllCompetitionForUser(string ownerId);
        CompetitionModel GetCompetitionById(string id);
        void Register(CompetitionModel competition);
        bool isCompetitionInDb(CompetitionModel competition);
        void CompetitionUpdate(CompetitionModel competition);
    }
}
