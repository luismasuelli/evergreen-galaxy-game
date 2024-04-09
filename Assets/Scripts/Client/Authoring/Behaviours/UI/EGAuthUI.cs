using System;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using Protocols.Messages;
using TMPro;
using Exception = AlephVault.Unity.Meetgard.Types.Exception;

namespace Client.Authoring.Behaviours.UI
{
    using Protocols;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class EGAuthUI : MonoBehaviour
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
        ///   A status label.
        /// </summary>
        [SerializeField]
        private TMP_Text statusLabel;
        
        /// <summary>
        ///   The submit button.
        /// </summary>
        [SerializeField]
        private Button submit;
        
        /// <summary>
        ///   The UI to show when online.
        /// </summary>
        [SerializeField]
        private GameObject onlineUI;

        /// <summary>
        ///   The button to redirect to the register UI.
        /// </summary>
        [SerializeField]
        private Button buttonToRegister;

        /// <summary>
        ///   The register UI.
        /// </summary>
        [SerializeField]
        private GameObject registerUI;
        
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
            
            if (!username || !password)
            {
                throw new Exception("The register form fields are not properly initialized!");
            }
            
            if (!submit)
            {
                throw new Exception("The submit button is not properly initialized!");
            }
        }
        
        private Task UseCanvas(bool online)
        {
            return protocol.RunInMainThread(() => {
                onlineUI.SetActive(online);
                // Please note: I changed the offlineUI for this same object.
                gameObject.SetActive(!online);
            });
        }
        
        private void Start()
        {
            if (buttonToRegister && registerUI) {
                buttonToRegister.onClick.AddListener(() => {
                    gameObject.SetActive(false);
                    registerUI.SetActive(true);
                });
            }
            submit.onClick.AddListener(OnSubmitClick);
            client.OnConnected += OnClientConnected;
            client.OnDisconnected += OnClientDisconnected;            
            protocol.OnLoginOK += OnLoginOK;
            protocol.OnLoginFailed += OnLoginFailed;
        }

        private void OnEnable()
        {
            SetStatus("Press \"Sign In\" to continue...");
        }

        private void OnDestroy()
        {
            submit.onClick.RemoveListener(OnSubmitClick);
            client.OnConnected -= OnClientConnected;
            client.OnDisconnected -= OnClientDisconnected;            
            protocol.OnLoginOK -= OnLoginOK;
            protocol.OnLoginFailed -= OnLoginFailed;
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
            UseCanvas(false);
        }
        
        private async Task OnLoginOK(Nothing _)
        {
            // Please note: The argument type must match the Auth protocol definition!
            SetStatus("Login was successful!");
            await UseCanvas(true);
        }
        
        private async Task OnLoginFailed(LoginFailed reason)
        {
            // Please note: The argument type must match the Auth protocol definition!
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
            SetStatus("Logging in...");
            protocol.Handshake.OnWelcome -= OnWelcome;
            await protocol.DefaultLoginSender(new Login() {
                Username = username.text,
                Password = password.text
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