using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class DateControl
    {
        public DateControl()
        {
            Effort = new EffortViewModel();
            Progress = new ProgressViewModel();
            Variance = new VarianceViewModel();
            DateControlType = new DateControlTypeViewModel();
        }
        public int Year { get; set; }
        public int Month { get; set; }
        [UIHint("ClientDateControlType")]
        public DateControlTypeViewModel DateControlType { get; set; }
        [UIHint("ClientEffort")]
        public EffortViewModel Effort { get; set; }
        [UIHint("ClientProgress")]
        public ProgressViewModel Progress { get; set; }
        [UIHint("ClientVariance")]
        public VarianceViewModel Variance { get; set; }
    }
    public class EffortViewModel
    {
        public int EffortId { get; set; }
        public string EffortName { get; set; }
    }
    public class ProgressViewModel
    {
        public int ProgressId { get; set; }
        public string ProgressName { get; set; }
    }
    public class VarianceViewModel
    {
        public int VarianceId { get; set; }
        public string VarianceName { get; set; }
    }
    public class DateControlTypeViewModel
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
