using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class MeasurementSimulationModel
    {
        public int EpicId { get; set; }
        public string EpicName { get; set; }
        public Module Module { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public float EpicWeight { get; set; }
        [UIHint("ClientTeam")]
        public Team Team { get; set; }
        public Progress Actual { get; set; }
        public Progress Target { get; set; }
        public float PreviousMonthCumulativeActualEffort { get; set; }
        [UIHint("ClientNumericTextBox")]
        public float ActualEffort { get; set; }
    }
}
