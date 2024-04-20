using System;
using Newtonsoft.Json;

namespace Server.Authoring.Types
{
    /// <summary>
    ///   The server config.
    /// </summary>
    [Serializable]
    public class ServerLauncherConfig
    {
        /// <summary>
        ///   The game port. By default, 7667.
        /// </summary>
        [JsonProperty("port")]
        public ushort Port;
    }
}