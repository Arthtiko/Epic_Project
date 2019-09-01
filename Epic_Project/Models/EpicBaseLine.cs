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
            TeamName = new Team();
        }
        [Required]
        public int EPICId { get; set; }
        [Required]
        public string EPICName { get; set; }
        [UIHint("ClientModule")]
        public Module ModuleName { get; set; }
        [UIHint("ClientEpicType")]
        public EpicTypeViewModel EpicType { get; set; }
        [UIHint("ClientProjectLocation")]
        public ProjectLocationViewModel ProjectLocation { get; set; }
        public float Estimation { get; set; }
        public float EpicWeight { get; set; }
        [UIHint("ClientTeam")]
        public Team TeamName { get; set; }
        [UIHint("ClientMurabaha")]
        public MurabahaViewModel IsMurabaha { get; set; }
        [UIHint("ClientFirstSellableModule")]
        public IsFirstSellableModuleViewModel IsFirstSellableModule { get; set; }
        public float DistributedUnmappedEffort { get; set; }
        public float ActualEffort { get; set; }
        public float TotalActualEffort { get; set; }
    }
}
