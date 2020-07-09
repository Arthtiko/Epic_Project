using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class Feature
    {
        public Feature()
        {
            FSM = new FeatureFSMModel();
            Team = new FeatureTeamModel();
        }
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public float FeatureEstimation { get; set; }
        [UIHint("ClientFeatureFSM")]
        public FeatureFSMModel FSM { get; set; }
        public int EpicId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TypeValue { get; set; }
        public string TypeName { get; set; }
        public float RequirementProgress { get; set; }
        public float DesignProgress { get; set; }
        public float DevelopmentProgress { get; set; }
        public float TestProgress { get; set; }
        public float UatProgress { get; set; }
        public float OverallEpicCompletion { get; set; }
        public float PreviousMonthCumulativeActualEffort { get; set; }
        public float ActualEffort { get; set; }
        public string UserName { get; set; }
        public string UserIp { get; set; }
        [UIHint("ClientFeatureTeam")]
        public FeatureTeamModel Team { get; set; }
    }

    public class FeatureReport
    {
        public FeatureReport()
        {
            
        }
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public float FeatureEstimation { get; set; }
        public string FSM { get; set; }
        public string Team { get; set; }
        public int EpicId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TypeValue { get; set; }
        public string TypeName { get; set; }
        public float CurrentRequirementProgress { get; set; }
        public float CurrentDesignProgress { get; set; }
        public float CurrentDevelopmentProgress { get; set; }
        public float CurrentTestProgress { get; set; }
        public float CurrentUatProgress { get; set; }
        public float CurrentOverallEpicCompletion { get; set; }
        public float PrevMonthRequirementProgress { get; set; }
        public float PrevMonthDesignProgress { get; set; }
        public float PrevMonthDevelopmentProgress { get; set; }
        public float PrevMonthTestProgress { get; set; }
        public float PrevMonthUatProgress { get; set; }
        public float PrevMonthOverallEpicCompletion { get; set; }
    }

    public class FeatureTeamModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }

    public class FeatureFSMModel
    {
        public int FSMValue { get; set; }
        public string FSMName { get; set; }
    }
    
}
