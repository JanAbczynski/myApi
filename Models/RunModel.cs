using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Models
{
    public class RunModel
    {
        [Key]
        public string id { get; set; }
        public string descriptiion { get; set; }
        public string target { get; set; }
        public int noOfShots { get; set; }
        
    }
}
