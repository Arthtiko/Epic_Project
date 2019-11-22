using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class Team
    {
        public Team()
        {
            TeamLeader = new TeamLeaderViewModel();
            ProjectManager = new ProjectManagerViewModel();
        }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        [UIHint("ClientTeamLeader")]
        public TeamLeaderViewModel TeamLeader { get; set; }
        [UIHint("ClientProjectManager")]
        public ProjectManagerViewModel ProjectManager { get; set; }
        public string TeamLocation { get; set; }
    }

    public class TeamLeaderViewModel
    {
        public int TeamLeaderId { get; set; }
        public string TeamLeaderName { get; set; }
    }

    public class ProjectManagerViewModel
    {
        public int ProjectManagerId { get; set; }
        public string ProjectManagerName { get; set; }
    }
}
