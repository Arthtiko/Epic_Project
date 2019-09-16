using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class Measurement
    {
        public Measurement()
        {
            Type = new MeasurementTypeViewModel();
            Module = new Module();
            Team = new Team();
        }
        [Required]
        public int EpicId { get; set; }
        public string EpicName { get; set; }
        [UIHint("ClientModule")]
        public Module Module { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [UIHint("clientmeasurementtype")]
        public MeasurementTypeViewModel Type { get; set; }
        public float EpicWeight { get; set; }
        [UIHint("clientteam")]
        public Team Team { get; set; }
        [UIHint("clientpercenttext")]
        public float RequirementProgress { get; set; }
        [UIHint("clientpercenttext")]
        public float DesignProgress { get; set; }
        [UIHint("clientpercenttext")]
        public float DevelopmentProgress { get; set; }
        [UIHint("clientpercenttext")]
        public float TestProgress { get; set; }
        [UIHint("clientpercenttext")]
        public float UatProgress { get; set; }
        public float PreviousMonthCumulativeActualEffort { get; set; }
        [UIHint("clientpercenttext")]
        public float ActualEffort { get; set; }
    }
}
