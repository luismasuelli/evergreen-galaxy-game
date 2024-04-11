using Core.Types.Characters;
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

        [JsonProperty("sex")]
        public SexType Sex;

        [JsonProperty("race")]
        public RaceType Race;

        [JsonProperty("hair_type")]
        public HairType Hair;

        [JsonProperty("hair_color")]
        public HairColorType HairColor;
    }
}
