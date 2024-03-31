using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.External.Models
{
    public class Map
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("scope_id")]
        public string ScopeId;
        
        [JsonProperty("index")]
        public int Index;

        [JsonProperty("drop")]
        public uint[][] Drop;
    }
}
