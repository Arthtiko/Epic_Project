﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class MeasurementSearchModel
    {
        public int EpicId { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        public string YearMonth { get; set; }
        public int NextMonth { get; set; }
        public int NextYear { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string TeamName { get; set; }
    }
}
