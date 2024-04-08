using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using Protocols.Messages;
using TMPro;

namespace Client.Authoring.Behaviours.UI
{
    using Protocols;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class EGActiveUI : MonoBehaviour
    {
        /// <summary>
        ///   The involved network client.
        /// </summary>
        [SerializeField]
        private NetworkClient client;

        /// <summary>
        ///   The button that closes the connection.
        /// </summary>
        [SerializeField]
        private Button closeButton;
        
        // The client's register protocol.
        private EGAuthProtocolClientSide protocol;
                 
        private void Awake()
        {
            if (!client)
            {
                throw new Exception("No network client is referenced in this object!");
            }
            protocol = client.GetComponent<EGAuthProtocolClientSide>();
            if (!protocol)
            {
                throw new Exception("The network protocol does not have a behaviour of instance " +
                                    "EGAuthProtocolClientSide attached to it");
            }
        }
        
        private void Start()
        {
            closeButton.onClick.AddListener(OnCloseClick);
        }
        
        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnCloseClick);
        }
        
        private void OnCloseClick()
        {
            _ = protocol.Logout();
        }
    }
}