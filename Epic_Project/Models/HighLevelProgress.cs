using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class HighLevelProgress
    {
        public string TaskBreakdown { get; set; }
        public int Finished { get; set; }
        public int InProgress { get; set; }
        public int InQueue { get; set; }
        public int Total { get; set; }
        public string TargetFinishDate { get; set; }
    }
}
