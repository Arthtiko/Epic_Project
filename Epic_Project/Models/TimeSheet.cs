using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class TimeSheet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Project { get; set; }
        public string Task { get; set; }
        public float Hour { get; set; }
        public DateTime Date { get; set; }
    }
}
