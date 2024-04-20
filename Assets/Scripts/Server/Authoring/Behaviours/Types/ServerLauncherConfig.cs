using System;
using Newtonsoft.Json;

namespace Server.Authoring.Behaviours.Types
{
    /// <summary>
    ///   The server config.
    /// </summary>
    [Serializable]
    public class ServerLauncherConfig
    {
        /// <summary>
        ///   The default port, if not specified.
        /// </summary>
        public const ushort DefaultPort = 7667;
        
        /// <summary>
        ///   The game port. By default, 7667.
        /// </summary>
        [JsonProperty("port")]
        public ushort Port;
    }
}