using Newtonsoft.Json;

namespace PluginSageIntacct.DataContracts
{
    public class ApiError
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}