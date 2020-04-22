using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class Module
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public float Progress { get; set; }
        public float ActualEffort { get; set; }
        public float Weight { get; set; }
        public float Variance { get; set; }
        public int EpicCount { get; set; }
        public float RequirementProgress { get; set; }
        public float DesignProgress { get; set; }
        public float DevelopmentProgress { get; set; }
        public float TestProgress { get; set; }
        public float UatProgress { get; set; }
        public float TotalEstimation { get; set; }
        public float WeightedOverallProgress { get; set; }
        public float FSMPercentage { get; set; }
        public string EpicString { get; set; }
        public string EpicCountFooter { get; set; }
    }

    public class ModuleModel
    {
        public float TotalEpicCount { get; set; }
    }
    
}
