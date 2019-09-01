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
            TeamLeader = new EmployeeViewModel();
            ProjectManager = new EmployeeViewModel();
        }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        [UIHint("ClientTeamLeader")]
        public EmployeeViewModel TeamLeader { get; set; }
        [UIHint("ClientProjectManager")]
        public EmployeeViewModel ProjectManager { get; set; }
    }
}
