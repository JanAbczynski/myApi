﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Models
{
    public class CompetitionModel
    {
        [Key]
        public string Id { get; set; }
        public string description { get; set; }
        //public RunModel[] runs { get; set; }
        public DateTime startTime { get; set; }
        public DateTime duration { get; set; }
        public string placeOf { get; set; }

    }
}
