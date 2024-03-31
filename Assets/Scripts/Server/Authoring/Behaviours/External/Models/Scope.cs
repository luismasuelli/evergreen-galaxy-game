using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.External.Models
{
    public class Scope
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("key")]
        public string Key;

        [JsonProperty("template_key")]
        public string PrefabKey;
    }
}
