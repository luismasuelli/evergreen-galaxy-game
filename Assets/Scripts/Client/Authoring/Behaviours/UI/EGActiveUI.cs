using System;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Client;
using Client.Authoring.Behaviours.NetworkObjects;
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

        /// <summary>
        ///   The close dialog.
        /// </summary>
        [SerializeField]
        private GameObject closeDialog;

        /// <summary>
        ///   The "No" button in the close dialog.
        /// </summary>
        [SerializeField]
        private Button closeNoButton;

        /// <summary>
        ///   The "Back to Lobby" button in the close dialog.
        /// </summary>
        [SerializeField]
        private Button closeLobbyButton;

        /// <summary>
        ///   The "Logout" button in the close dialog.
        /// </summary>
        [SerializeField]
        private Button closeLogoutButton;

        /// <summary>
        ///   The error dialog.
        /// </summary>
        [SerializeField]
        private GameObject errorDialog;

        /// <summary>
        ///   The error dialog text.
        /// </summary>
        [SerializeField]
        private TMP_Text errorDialogText;

        /// <summary>
        ///   The error dialog close button.
        /// </summary>
        [SerializeField]
        private Button errorOkButton;

        /// <summary>
        ///   The dialog to select a character.
        /// </summary>
        [SerializeField]
        private GameObject selectCharacterDialog;

        /// <summary>
        ///   The dialog to create a character.
        /// </summary>
        [SerializeField]
        private GameObject createCharacterDialog;

        // The auth protocol.
        private EGAuthProtocolClientSide authProtocol;
        
        // The player protocol.
        private NetRoseProtocolClientSide playerProtocol;

        // The game protocol.
        private EGGameProtocolClientSide gameProtocol;
                 
        private void Awake()
        {
            if (!client)
            {
                throw new Exception("No network client is referenced in this object!");
            }
            if (!gameProtocol)
            {
                throw new Exception("A game protocol must be assigned");
            }

            gameProtocol = client.GetComponent<EGGameProtocolClientSide>();
            if (!gameProtocol)
            {
                throw new Exception("The client does not have a game protocol");
            }

            authProtocol = gameProtocol.GetComponent<EGAuthProtocolClientSide>();
            if (!authProtocol)
            {
                throw new Exception("The client does not have an auth protocol");
            }

            playerProtocol = GetComponent<NetRoseProtocolClientSide>();
            if (!playerProtocol)
            {
                throw new Exception("The client does not have a NetRose protocol");
            }
        }
        
        private void Start()
        {
            closeButton.onClick.AddListener(OnCloseClick);
            closeLogoutButton.onClick.AddListener(OnCloseLogoutClick);
            closeLobbyButton.onClick.AddListener(OnCloseLobbyClick);
            closeNoButton.onClick.AddListener(OnCloseNoClick);
            errorOkButton.onClick.AddListener(OnErrorOkClick);
            gameProtocol.OnCharacterCreated += OnCharacterCreated;
            gameProtocol.OnCharacterCreateError += OnCharacterCreateError;
            gameProtocol.OnCharacterPickOk += OnCharacterPicked;
            gameProtocol.OnCharacterPickError += OnCharacterPickError;
            gameProtocol.OnCharacterReleaseOk += OnCharacterReleased;
            gameProtocol.OnCharacterReleaseError += OnCharacterReleaseError;
            CharacterClientSide.OnPlayerCharacterSpawned += OnPlayerCharacterSpawned;
        }
        
        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnCloseClick);
            closeLogoutButton.onClick.RemoveListener(OnCloseLogoutClick);
            closeLobbyButton.onClick.RemoveListener(OnCloseLobbyClick);
            closeNoButton.onClick.RemoveListener(OnCloseNoClick);
            errorOkButton.onClick.RemoveListener(OnErrorOkClick);
            gameProtocol.OnCharacterCreated -= OnCharacterCreated;
            gameProtocol.OnCharacterCreateError -= OnCharacterCreateError;
            gameProtocol.OnCharacterPickOk -= OnCharacterPicked;
            gameProtocol.OnCharacterPickError -= OnCharacterPickError;
            gameProtocol.OnCharacterReleaseOk -= OnCharacterReleased;
            gameProtocol.OnCharacterReleaseError -= OnCharacterReleaseError;
            CharacterClientSide.OnPlayerCharacterSpawned -= OnPlayerCharacterSpawned;
        }

        private void ShowError(string text)
        {
            errorDialogText.text = text;
            errorDialog.SetActive(true);
        }

        private void OnEnable()
        {
            closeDialog.SetActive(false);
            errorDialog.SetActive(false);
            selectCharacterDialog.SetActive(true);
        }

        private async Task OnCharacterCreateError(CharacterCreateError error)
        {
            await gameProtocol.RunInMainThread(async () =>
            {
                switch (error.Code)
                {
                    case CharacterCreateError.CharacterCreateErrorCode.AlreadyPicked:
                        ShowError("Cannot create a character while a character is being picked right now");
                        break;
                    case CharacterCreateError.CharacterCreateErrorCode.InvalidData:
                        ShowError("Invalid data. Ensure all the fields are set and the name is at most " +
                                  "20 alphanumeric/_ characters");
                        break;
                    case CharacterCreateError.CharacterCreateErrorCode.MaxCharactersReached:
                        ShowError("You have created all the allowed characters for your account");
                        break;
                    case CharacterCreateError.CharacterCreateErrorCode.DisplayNameInUse:
                        ShowError("The display name is already in use by another character");
                        break;
                    default:
                        ShowError("An error occurred while creating a character");
                        break;
                }
            });
        }

        private async Task OnCharacterPickError(CharacterPickError error)
        {
            await gameProtocol.RunInMainThread(async () =>
            {
                switch (error.Code)
                {
                    case CharacterPickError.CharacterPickErrorCode.AlreadyPicked:
                        ShowError("A character is already picked");
                        break;
                    case CharacterPickError.CharacterPickErrorCode.InvalidIndex:
                        ShowError("The selected character does not exist");
                        break;
                    default:
                        ShowError("An error occurred while selecting a character");
                        break;
                }
            });
        }

        private async Task OnCharacterReleaseError()
        {
            await gameProtocol.RunInMainThread(async () =>
            {
                ShowError("No character is currently selected");
            });

        }

        private void OnPlayerCharacterSpawned()
        {
            gameProtocol.RunInMainThread(() =>
            {
                closeDialog.SetActive(false);
                errorDialog.SetActive(false);
                selectCharacterDialog.SetActive(false);
                createCharacterDialog.SetActive(false);
            });
        }

        private async Task OnCharacterCreated()
        {
            await gameProtocol.RunInMainThread(() =>
            {
                selectCharacterDialog.SetActive(false);
                createCharacterDialog.SetActive(false);
            });
        }

        private async Task OnCharacterPicked()
        {
            await gameProtocol.RunInMainThread(() =>
            {
                selectCharacterDialog.SetActive(false);
                createCharacterDialog.SetActive(false);
            });
        }

        private async Task OnCharacterReleased()
        {
            await gameProtocol.RunInMainThread(() =>
            {
                selectCharacterDialog.SetActive(true);
            });
        }

        private void OnErrorOkClick()
        {
            errorDialog.SetActive(false);
        }
        
        private void OnCloseLogoutClick()
        {
            _ = authProtocol.Logout();
            closeDialog.SetActive(false);
        }

        private void OnCloseLobbyClick()
        {
            _ = gameProtocol.CharacterRelease();
            closeDialog.SetActive(false);
        }

        private void OnCloseNoClick()
        {
            closeDialog.SetActive(false);
        }

        private void OnCloseClick()
        {
            closeDialog.SetActive(true);
        }
    }
}