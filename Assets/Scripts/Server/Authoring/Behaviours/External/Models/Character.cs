using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.External.Models
{
    public class Character
    {
        public enum SexType
        {
            Male = 0,
            Female = 1
        }

        public enum RaceType
        {
            White = 0,
            Brown = 1,
            Black = 2
        }

        public enum HairType
        {
            Short = 0,
            Middle = 1,
            Long = 2
        }

        public enum HairColorType
        {
            Brunette,
            Brown,
            Blonde
        }
        
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
