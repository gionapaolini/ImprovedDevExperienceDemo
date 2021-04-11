using System.Collections.Generic;

namespace Models
{
    public class QAModel
    {
        public string projectId { get; set; }

        public string data { get; set; }
    }
    public class IntentProjectData
    {
        public string projectId { get; set; }
        public List<IntentData> data { get; set; }
    }
    public class IntentData 
    {
        public string intent { get; set; }
        public List<string> sentences { get; set; }
    }
}