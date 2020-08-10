using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SurveyBE.Models 
{

	public class TaskAnswer
    {
        public string taskid { get; set; }
        public string taskname { get; set; }
        public string answer { get; set; }

    }

    public class SurveyEntryModel : TableEntity
    {
        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public string Q1Answer { get; set; }
		public string Q2Answer { get; set; }
		public string Q3Answer { get; set; }
		public string Q4Answer { get; set; }
		public string Q5EaseOfUseAnswer { get; set; }
        public string Q5FeaturesAndCapabilitiesAnswer { get; set; }
        public string Q5SpeedAnswer { get; set; }
        public string Q5TechnicalReliabilityAnswer { get; set; }
        public string Q5VisualAppealAnswer { get; set; }
        [IgnoreProperty]
	    public List<TaskAnswer> Q6Answer { get; set; }
        public string Q6AnswerJson{ get; set; }
        public string Q7Answer { get; set; }
		public string Q8Answer { get; set; }
    }
}
