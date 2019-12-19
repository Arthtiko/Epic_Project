using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class MeasurementDetailsViewModel
    {
        public MeasurementDetailsViewModel()
        {
            Module = new Module();
            Team = new Team();
        }
        [Required]
        public int EpicId { get; set; }
        public string EpicName { get; set; }
        public Module Module { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        public string Location { get; set; }
        public float EpicWeight { get; set; }
        public float Estimation { get; set; }
        public Team Team { get; set; }
        public string IsFirstSellableModule { get; set; }
        public float PrevMonthRequirementProgress { get; set; }
        public float PrevMonthDesignProgress { get; set; }
        public float PrevMonthDevelopmentProgress { get; set; }
        public float PrevMonthTestProgress { get; set; }
        public float PrevMonthUatProgress { get; set; }
        public float PrevMonthOverallEpicCompilation { get; set; }
        public float PrevMonthWeightedOverallProgress { get; set; }
        public float TargetRequirementProgress { get; set; }
        public float TargetDesignProgress { get; set; }
        public float TargetDevelopmentProgress { get; set; }
        public float TargetTestProgress { get; set; }
        public float TargetUatProgress { get; set; }
        public float TargetOverallEpicCompilation { get; set; }
        public float TargetWeightedOverallProgress { get; set; }
        public float ActualRequirementProgress { get; set; }
        public float ActualDesignProgress { get; set; }
        public float ActualDevelopmentProgress { get; set; }
        public float ActualTestProgress { get; set; }
        public float ActualUatProgress { get; set; }
        public float ActualOverallEpicCompilation { get; set; }
        public float ActualWeightedOverallProgress { get; set; }
        public float PreviousMonthCumulativeActualEffort { get; set; }
        public float ActualEffort { get; set; }
        public float Variance { get; set; }
    }
}
