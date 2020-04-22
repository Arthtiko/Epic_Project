using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class ProgressProducingVarianceAnalysis
    {
        public ProgressProducingVarianceAnalysis()
        {
            Team = new Team();
        }
        public Team Team { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int ResourceCount { get; set; }
        public int PlannedManday { get; set; }
        public int DayOff { get; set; }
        public float PlannedConsumedMandayBudget { get; set; }
        public float ThresholdIncrementProgress { get; set; }
        public float PreviousMonthOverallProgress { get; set; }
        public float TargetProgress { get; set; }
        public float IncrementProgress { get; set; }
        public float Difference { get; set; }
        public float Variance { get; set; }
    }

    public class NonProgressProducingVarianceAnalysis
    {
        public NonProgressProducingVarianceAnalysis()
        {
            Team = new Team();
        }
        public Team Team { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int NonProgressProducingResourceCount { get; set; }
        public int ResourceCount { get; set; }
        public int TotalResourceCount { get; set; }
        public int PlannedManday { get; set; }
        public int DayOff { get; set; }
        public float PlannedConsumedMandayBudget { get; set; }
        public float ThresholdIncrementProgress { get; set; }
        public float PreviousMonthOverallProgress { get; set; }
        public float TargetProgress { get; set; }
        public float IncrementProgress { get; set; }
        public float Difference { get; set; }
        public float Variance { get; set; }
    }

    public class VarianceAnalysis
    {
        public int TeamId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int ProgressProducingResourceCount { get; set; }
        public int NonProgressProducingResourceCount { get; set; }
        public int ProgressProducingResourceDayOff { get; set; }
        public int NonProgressProducingResourceDayOff { get; set; }
        public float TargetProgress { get; set; }
    }
}
