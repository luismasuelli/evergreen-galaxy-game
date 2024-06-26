using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using AlephVault.Unity.Support.Utils;
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
        
        /// <summary>
        ///   The button to redirect to the authenticate UI.
        /// </summary>
        [SerializeField]
        private Button buttonToAuthenticate;

        /// <summary>
        ///   The authenticate UI.
        /// </summary>
        [SerializeField]
        private GameObject authenticateUI;

        // The client's register protocol.
        private EGRegisterProtocolClientSide protocol;

        // Flag to tell whether a registration was successful.
        // This prevents immediately restoring the buttons in
        // that case, immediately, but will be done later, in
        // a delayed manner.
        private bool wasSuccessfulRegistration = false;
                 
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
            if (buttonToAuthenticate && authenticateUI) {
                buttonToAuthenticate.onClick.AddListener(() => {
                    gameObject.SetActive(false);
                    authenticateUI.SetActive(true);
                });
            }
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
        
        private void OnEnable()
        {
            SetStatus("Press \"Register\" to continue...");
        }
        
        private void OnClientConnected()
        {
            protocol.RunInMainThread(() =>
            {
                submit.interactable = false;
                buttonToAuthenticate.interactable = false;
            });
        }
        
        private void OnClientDisconnected()
        {
            protocol.RunInMainThread(() =>
            {
                protocol.Handshake.OnWelcome -= OnWelcome;
                protocol.Handshake.OnTimeout -= OnTimeout;
                if (wasSuccessfulRegistration)
                {
                    wasSuccessfulRegistration = false;
                }
                else
                {
                    submit.interactable = true;
                    buttonToAuthenticate.interactable = true;
                }
            });
        }
        
        private async Task OnRegisterOK(Nothing _)
        {
            // Please note: The argument type must match the Register protocol definition!
            SetStatus("Register was successful!");
            await protocol.RunInMainThread(async () =>
            {
                if (buttonToAuthenticate) buttonToAuthenticate.interactable = false;
                if (submit) submit.interactable = false;
                float timeout = 0;
                while (timeout < 2f)
                {
                    await Tasks.Blink();
                    timeout += Time.deltaTime;
                }
                gameObject.SetActive(false);
                if (authenticateUI)
                {
                    authenticateUI.SetActive(true);
                    authenticateUI.GetComponent<EGAuthUI>().PrepopulateCredentials(
                        username.text,
                        password.text
                    );
                }
                if (buttonToAuthenticate) buttonToAuthenticate.interactable = true;
                if (submit) submit.interactable = true;
            });
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