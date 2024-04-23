using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using Server.Authoring.Behaviours.Types;
using UnityEngine;

namespace Server.Authoring.Behaviours.Protocols
{
    [RequireComponent(typeof(NetworkServer))]
    public class ServerLauncher : MonoBehaviour
    {
        private NetworkServer server;

        private void Awake()
        {
            server = GetComponent<NetworkServer>();
        }

        // Start is called before the first frame update
        void Start()
        {
            ServerLauncherConfig config;
            try
            {
                config = JsonUtility.FromJson<ServerLauncherConfig>("config.json");
            }
            catch
            {
                Debug.Log("Using the default configuration since no config.json file is present");
                config = new ServerLauncherConfig
                {
                    // TODO add a default config.
                };
            }
            // TODO use this config later.
            server.StartServer(6777);
        }
    }
}
