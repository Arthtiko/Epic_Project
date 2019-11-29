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
    public class LineChartModel2
    {
        public double[] ActualEffort { get; set; }
        public double[] OverallCompilation { get; set; }
        public double[] Variance { get; set; }
        public string[] Category { get; set; }
        public LineChartModel2()
        {

        }
    }
}
