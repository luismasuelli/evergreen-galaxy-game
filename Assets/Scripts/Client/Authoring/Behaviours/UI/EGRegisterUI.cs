using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using Protocols.Messages;
using TMPro;

namespace Client.Authoring.Behaviours.UI
{
    using Protocols;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class EGRegisterUI : MonoBehaviour
    {
        /// <summary>
        ///   The involved network client.
        /// </summary>
        [SerializeField]
        private NetworkClient client;
        
        /// <summary>
        ///   The address to connect to.
        /// </summary>
        [SerializeField]
        private string address = "localhost";

        /// <summary>
        ///   The port to connect to.
        /// </summary>
        [SerializeField]
        private ushort port = 6777;

        /// <summary>
        ///   The username field.
        /// </summary>
        [SerializeField]
        private TMP_InputField username;

        /// <summary>
        ///   The password field.
        /// </summary>
        [SerializeField]
        private TMP_InputField password;

        /// <summary>
        ///   The password confirm field.
        /// </summary>
        [SerializeField]
        private TMP_InputField passwordConfirm;
        
        /// <summary>
        ///   A status label.
        /// </summary>
        [SerializeField]
        private TMP_Text statusLabel;

        /**
         * Uncomment this field if the register involves some sort
         * of display name for the registered accounts, instead of
         * a separate concept / data type.
         *
         * /// <summary>
         * ///   The display name field.
         * /// </summary>
         * [SerializeField]
         * private InputField displayName;
         */
        
        /// <summary>
        ///   The submit button.
        /// </summary>
        [SerializeField]
        private Button submit;
        
        // The client's register protocol.
        private EGRegisterProtocolClientSide protocol;
                 
        private void Awake()
        {
            if (!client)
            {
                throw new Exception("No network client is referenced in this object!");
            }
            protocol = client.GetComponent<EGRegisterProtocolClientSide>();
            if (!protocol)
            {
                throw new Exception("The network protocol does not have a behaviour of instance " +
                                    "EVRegisterProtocolClientSide attached to it");
            }
            
            // Remove the comment on the displayName variable if it is used by this form.
            if (!username || !password || !passwordConfirm /* || displayName */)
            {
                throw new Exception("The register form fields are not properly initialized!");
            }
            
            if (!submit)
            {
                throw new Exception("The submit button is not properly initialized!");
            }
        }
        
        private void Start()
        {
            submit.onClick.AddListener(OnSubmitClick);
            client.OnConnected += OnClientConnected;
            client.OnDisconnected += OnClientDisconnected;            
            protocol.OnRegisterOK += OnRegisterOK;
            protocol.OnRegisterFailed += OnRegisterFailed;
        }
        
        private void OnDestroy()
        {
            submit.onClick.RemoveListener(OnSubmitClick);
            client.OnConnected -= OnClientConnected;
            client.OnDisconnected -= OnClientDisconnected;            
            protocol.OnRegisterOK -= OnRegisterOK;
            protocol.OnRegisterFailed -= OnRegisterFailed;
        }
        
        private void OnClientConnected()
        {
            submit.interactable = false;
        }
        
        private void OnClientDisconnected()
        {
            protocol.Handshake.OnWelcome -= OnWelcome;
            protocol.Handshake.OnTimeout -= OnTimeout;
            submit.interactable = true;
        }
        
        private async Task OnRegisterOK(Nothing _)
        {
            // Please note: The argument type must match the Register protocol definition!
            SetStatus("Register was successful!");
        }
        
        private async Task OnRegisterFailed(RegisterFailed reason)
        {
            // Please note: The argument type must match the Register protocol definition!
            SetStatus(reason.Reason);
        }
        
        private void OnSubmitClick()
        {
            submit.interactable = false;
            try
            {
                SetStatus("Connecting...");
                protocol.Handshake.OnWelcome += OnWelcome;
                protocol.Handshake.OnTimeout += OnTimeout;
                client.Connect(address, port);
            }
            catch(System.Exception)
            {
                SetStatus("Connection error!");
                protocol.Handshake.OnWelcome -= OnWelcome;
                protocol.Handshake.OnTimeout -= OnTimeout;
                submit.interactable = true;
            }
        }
        
        private async Task OnWelcome()
        {
            SetStatus("Registering...");
            protocol.Handshake.OnWelcome -= OnWelcome;
            await protocol.DefaultRegisterSender(new Register() {
                Username = username.text,
                Password = password.text,
                PasswordConfirm = passwordConfirm.text,
                // Uncomment the following line if your register makes use of DisplayName.
                /* DisplayName = displayName.text */
            });
        }
        
        private async Task OnTimeout()
        {
            SetStatus("Handshake timeout!");            
        }
        
        private void SetStatus(string value)
        {
            if (statusLabel) {
                protocol.RunInMainThread(() => {
                    statusLabel.text = value;
                });
            }
        }
    }
}