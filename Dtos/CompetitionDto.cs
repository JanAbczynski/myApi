using Comander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Dtos
{
    public class CompetitionDto
    {

        public string Id { get; set; }
        public string description { get; set; }
        //public RunModel[] runs { get; set; }
        public DateTime startTime { get; set; }
        public DateTime duration { get; set; }
        public string placeOf { get; set; }
    }
}
