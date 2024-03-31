using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.External.Models
{
    public class Character
    {
        [JsonProperty("_id")]
        public string Id;
        
        [JsonProperty("account_id")]
        public string AccountId;

        [JsonProperty("display_name")]
        public string DisplayName;
        
        [JsonProperty("position")]
        public Position Position;
    }
}
