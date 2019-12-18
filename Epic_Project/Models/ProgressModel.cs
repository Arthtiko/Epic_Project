using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class ProgressModel
    {
        public float Completed { get; set; }
        public float Total { get; set; }
        public float ActualEffort { get; set; }
        public float? Variance { get; set; }

        public ProgressModel(){ }
        public ProgressModel(float completed, float total, float actualEffort, float variance)
        {
            Completed = completed;
            Total = total;
            ActualEffort = actualEffort;
            Variance = variance;
        }
    }

    public class LineChartModel
    {
        public float? ActualEffort { get; set; }
        public float? OverallCompilation { get; set; }
        public float? Variance { get; set; }
        public string Category { get; set; }
        public LineChartModel()
        {

        }
    }
    public class GraphControl
    {
        public bool ShowActualEffort { get; set; }
        public bool ShowProgress { get; set; }
        public bool ShowVariance { get; set; }

        public GraphControl()
        {
            ShowActualEffort = true;
            ShowProgress = true;
            ShowVariance = true;
        }
    }
}
