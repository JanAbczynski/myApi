using Comander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Data
{
    public interface IRunRepo
    {
        bool SaveChanges();
        void Register(RunModel run);
        IEnumerable<RunModel> GetAllRuns();
        IEnumerable<RunModel> GetRunByCompetitionId(string competitionId);

    }
}
