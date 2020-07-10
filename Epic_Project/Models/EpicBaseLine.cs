using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class EpicBaseLine
    {
        public EpicBaseLine()
        {
            EpicType = new EpicTypeViewModel();
            ModuleName = new Module();
            IsMurabaha = new MurabahaViewModel();
            IsFirstSellableModule = new IsFirstSellableModuleViewModel();
            ProjectLocation = new ProjectLocationViewModel();
            Team = new Team();
            EditMode = new EditModeModel();
        }
        [Required]
        [UIHint("NoSpinners")]
        public int EPICId { get; set; }
        [Required]
        public string EPICName { get; set; }
        public Module ModuleName { get; set; }
        public EpicTypeViewModel EpicType { get; set; }
        [UIHint("ClientProjectLocation")]
        public ProjectLocationViewModel ProjectLocation { get; set; }
        public float Estimation { get; set; }
        public float FSMPercentage { get; set; }
        public float EpicWeight { get; set; }
        [UIHint("ClientTeam")]
        public Team Team { get; set; }
        [UIHint("ClientMurabaha")]
        public MurabahaViewModel IsMurabaha { get; set; }
        [UIHint("ClientFirstSellableModule")]
        public IsFirstSellableModuleViewModel IsFirstSellableModule { get; set; }
        public float DistributedUnmappedEffort { get; set; }
        public float ActualEffort { get; set; }
        public float TotalActualEffort { get; set; }
        public string Description { get; set; }
        public string Dependency { get; set; }
        [UIHint("ClientEditMode")]
        public EditModeModel EditMode { get; set; }
    }

    public class EpicTypeViewModel
    {
        public int TypeValue { get; set; }
        public string TypeName { get; set; }
    }

    public class ProjectLocationViewModel
    {
        public int LocationValue { get; set; }
        public string LocationName { get; set; }
    }

    public class MurabahaViewModel
    {
        public int MurabahaValue { get; set; }
        public string MurabahaName { get; set; }
    }

    public class IsFirstSellableModuleViewModel
    {
        public int FirstSellableModuleValue { get; set; }
        public string FirstSellableModuleName { get; set; }
    }
}
