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
        public Module Module { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [UIHint("ClientMeasurementType")]
        public MeasurementTypeViewModel Type { get; set; }
        public float EpicWeight { get; set; }
        [UIHint("ClientTeam")]
        public Team Team { get; set; }
        public string IsFirstSellableModule { get; set; }
        [UIHint("ClientPercentText")]
        public float RequirementProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float DesignProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float DevelopmentProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float TestProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float UatProgress { get; set; }
        public float OverallEpicCompilation { get; set; }
        public float WeightedOverallProgress { get; set; }
        public float PreviousMonthCumulativeActualEffort { get; set; }
        [UIHint("ClientNumericTextBox")]
        public float ActualEffort { get; set; }
    }

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
        public int LastMonth { get; set; }
        public int LastYear { get; set; }
        public string UserId { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string TeamName { get; set; }
        public bool IsAllShowedTurkeyO { get; set; }
        public bool IsAllShowedTurkeyFSM { get; set; }
        public bool IsAllShowedEgyptO { get; set; }
        public bool IsAllShowedEgyptFSM { get; set; }
        public bool IsAllShowedTotalO { get; set; }
        public bool IsAllShowedTotalFSM { get; set; }
    }

    public class MeasurementTypeViewModel
    {
        public int TypeValue { get; set; }
        public string TypeName { get; set; }
    }

    public class Date
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class Progress
    {
        [UIHint("ClientPercentText")]
        public float RequirementProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float DesignProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float DevelopmentProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float TestProgress { get; set; }
        [UIHint("ClientPercentText")]
        public float UatProgress { get; set; }

        public Progress()
        {

        }
    }
}
