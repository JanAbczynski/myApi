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
        CompetitionModel GetCompetitionById(string id);
        void Register(CompetitionModel competition);
        bool isCompetitionInDb(CompetitionModel competition);
        void CompetitionUpdate(CompetitionModel competition);
    }
}
