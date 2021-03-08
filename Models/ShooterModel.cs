using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Models
{
    public class ShooterModel
    {
        [Key]
        public string id { get; set; }
        public string shooterId { get; set; }
        public string runId { get; set; }
    }
}
