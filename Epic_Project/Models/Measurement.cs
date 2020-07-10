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
            EditMode = new EditModeModel();
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
        public float FSMPercentage { get; set; }
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
        [UIHint("ClientEditMode")]
        public EditModeModel EditMode { get; set; }
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
        public int TeamId { get; set; }
        public int MaxYear { get; set; }
        public int MaxMonth { get; set; }
        public GraphControl TurkeyOverall { get; set; }
        public GraphControl EgyptOverall { get; set; }
        public GraphControl TotalOverall { get; set; }
        public GraphControl TurkeyFSM { get; set; }
        public GraphControl EgyptFSM { get; set; }
        public GraphControl TotalFSM { get; set; }
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

    public class MeasurementImportModel
    {
        public int EpicId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Type { get; set; }
        public float RequirementProgress { get; set; }
        public float DesignProgress { get; set; }
        public float DevelopmentProgress { get; set; }
        public float TestProgress { get; set; }
        public float UatProgress { get; set; }
        public float ActualEffort { get; set; }
    }

    public class ImportSearchModel
    {
        public int Mode { get; set; }
    }

    public class EditModeModel
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
