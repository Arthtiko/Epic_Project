using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class Finance
    {
        public string Category { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public float PeriodBudget { get; set; }
        public float TotalBudget { get; set; }
        public float Actual { get; set; }
    }

    public class FinanceReport
    {
        public string Category { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public float PeriodBudget { get; set; }
        public float Actual { get; set; }
        public float PeriodActualPercentage { get; set; }
        public float TotalBudget { get; set; }
        public float TotalRemaining { get; set; }
        public float TotalActualPercentage { get; set; }
    }

    public class FinanceSearchModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string YearMonth { get; set; }
        public bool IsTurkey { get; set; }
        public float PeriodActualPercentage { get; set; }
        public float TotalActualPercentage { get; set; }
    }

    public class FinanceGraph
    {
        public float? Line1 { get; set; }
        public float? Line2 { get; set; }
        public float? Line3 { get; set; }
        public float? Line4 { get; set; }
        public float? Line5 { get; set; }
        public float? Line6 { get; set; }
        public string Category { get; set; }
        public FinanceGraph()
        {

        }
    }
}
