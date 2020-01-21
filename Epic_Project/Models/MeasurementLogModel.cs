using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class MeasurementLogModel
    {
        public MeasurementLogModel()
        {
            
        }
        public int EpicId { get; set; }
        public string EpicName { get; set; }
        public string Module { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Type { get; set; }
        public string Team { get; set; }
        public float RequirementProgress { get; set; }
        public float DesignProgress { get; set; }
        public float DevelopmentProgress { get; set; }
        public float TestProgress { get; set; }
        public float UatProgress { get; set; }
        public float PreviousMonthCumulativeActualEffort { get; set; }
        public float ActualEffort { get; set; }
        public string UserName { get; set; }
        public string Time { get; set; }
        public string UserIp { get; set; }
    }
}
