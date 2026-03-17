using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace SampleCodeAPI.Model
{
    public class UserModels
    {
    }
    public class UserJWT
    {
        public string _id { get; set; }
        public string UserName { get; set; }
        public CustomData customdata { get; set; }
        public int staffID { get; set; }
        public long customerID { get; set; }
    }
    public class CustomData
    {
        public PersonalInfo personalInfo { get; set; }
        [JsonPropertyName("jee-account")]
        [JsonProperty("jee-account")]
        public JeeAccount jeeAccount { get; set; }
        [JsonPropertyName("role-hr")]
        [JsonProperty("role-hr")]
        public JeeHR RoleHR { get; set; }
    }

    public class JeeAccount
    {
        public string customerID { get; set; }
        public object appCode { get; set; }
        public int userID { get; set; }
        public int staffID { get; set; }
    }

    public class PersonalInfo
    {
        public string Avatar { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Jobtitle { get; set; }
        public string Department { get; set; }
        public string Birthday { get; set; }
        public string Phonenumber { get; set; }
        public string Fullname { get; set; }
    }
    public class JeeHR
    {
        public string roles { get; set; }
    }
}
