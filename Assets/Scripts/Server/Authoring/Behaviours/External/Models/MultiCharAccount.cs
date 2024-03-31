using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.External.Models
{
    public class MultiCharAccount
    {
        [JsonProperty("_id")]
        public string Id;
        
        [JsonProperty("login")]
        public string Login;
        
        // Hopefully, this password will be encrypted or hashed!
        [JsonProperty("password")]
        public string Password;
    }
}
