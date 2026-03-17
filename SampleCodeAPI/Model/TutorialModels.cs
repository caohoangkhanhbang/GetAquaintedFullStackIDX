using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace SampleCodeAPI.Model
{
    public class TutorialModels
    {
        public int RowID { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
    }
}
