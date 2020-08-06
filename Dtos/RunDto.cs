using Comander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Dtos
{
    public class RunDto
    {
        public string Id { get; set; }
        public string competitionId { get; set; }
        public string ownerId { get; set; }
        public string description { get; set; }
        public string target { get; set; }
        public int noOfShots { get; set; }
    }
}
