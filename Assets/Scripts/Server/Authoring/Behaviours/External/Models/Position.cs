using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.External.Models
{
    public class Position
    {
        [JsonProperty("scope")]
        public string Scope;
        
        [JsonProperty("map")]
        public int Map;

        [JsonProperty("x")]
        public ushort X;
        
        [JsonProperty("y")]
        public ushort Y;
    }
}
