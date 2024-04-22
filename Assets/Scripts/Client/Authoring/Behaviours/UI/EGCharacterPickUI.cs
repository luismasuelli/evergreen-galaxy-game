using System;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using Protocols.Messages;
using TMPro;
using Exception = System.Exception;

namespace Client.Authoring.Behaviours.UI
{
    using Protocols;
    using UnityEngine.UI;

    /// <summary>
    ///   A UI to pick a character.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class EGCharacterPickUI : MonoBehaviour
    {
        /// <summary>
        ///   The involved network client.
        /// </summary>
        [SerializeField]
        private NetworkClient client;
        
        // The game protocol.
        private EGGameProtocolClientSide gameProtocol;

        /// <summary>
        ///   The character create UI.
        /// </summary>
        [SerializeField]
        private GameObject characterCreateUI;

        /// <summary>
        ///   The involved "Register Character" button.
        /// </summary>
        [SerializeField]
        private Button goCharacterCreateButton;
        
        /// <summary>
        ///   The "Refresh" button.
        /// </summary>
        [SerializeField]
        private Button refreshButton;

        /// <summary>
        ///   The pane with buttons.
        /// </summary>
        [SerializeField]
        private GameObject buttonsPane;
        
        // The buttons in the pane.
        private Button[] buttons;
        
        /// <summary>
        ///   The status text.
        /// </summary>
        [SerializeField]
        private TMP_Text statusText;

        /// <summary>
        ///   The pane without buttons.
        /// </summary>
        [SerializeField]
        private GameObject emptyPane;

        private void Awake()
        {
            if (!client)
            {
                throw new Exception("No client is set");
            }
            gameProtocol = client.GetComponent<EGGameProtocolClientSide>();
            if (!gameProtocol)
            {
                throw new Exception("No game protocol is set in the client");
            }

            if (!characterCreateUI)
            {
                throw new Exception("No character create UI is set");
            }
            if (!goCharacterCreateButton)
            {
                throw new Exception("No 'Create Character' button is set");
            }
            if (!refreshButton)
            {
                throw new Exception("No 'Refresh' button is set");
            }
            if (!buttonsPane)
            {
                throw new Exception("No buttons pane is set");
            }
            buttons = buttonsPane.GetComponentsInChildren<Button>();

            if (!emptyPane)
            {
                throw new Exception("No empty pane is set");
            }
            if (!statusText)
            {
                throw new Exception("No status text is set");
            }
        }

        private void Start()
        {
            gameProtocol.OnCharacterReleaseOk += OnCharacterReleased;
            gameProtocol.OnCharacterListOk += OnCharacterListOk;
            gameProtocol.OnCharacterListError += OnCharacterListError;
            gameProtocol.OnCharacterPickOk += OnCharacterPickOk;
            gameProtocol.OnCharacterPickError += OnCharacterPickError;
            goCharacterCreateButton.onClick.AddListener(GoCharacterCreate);
            refreshButton.onClick.AddListener(DoRefresh);
        }

        private void OnDestroy()
        {
            gameProtocol.OnCharacterReleaseOk -= OnCharacterReleased;
            gameProtocol.OnCharacterListOk -= OnCharacterListOk;
            gameProtocol.OnCharacterListError -= OnCharacterListError;
            gameProtocol.OnCharacterPickOk -= OnCharacterPickOk;
            gameProtocol.OnCharacterPickError -= OnCharacterPickError;
            goCharacterCreateButton.onClick.RemoveListener(GoCharacterCreate);
            refreshButton.onClick.RemoveListener(DoRefresh);
        }

        private void OnEnable()
        {
            emptyPane.SetActive(false);
            buttonsPane.SetActive(false);
            foreach(Button button in buttons) button.gameObject.SetActive(false);
            DoRefresh();
        }

        private void GoCharacterCreate()
        {
            gameObject.SetActive(false);
            characterCreateUI.SetActive(true);
        }
        
        private async void DoRefresh()
        {
            await gameProtocol.CharacterList();
        }

        private async Task OnCharacterReleased()
        {
            gameObject.SetActive(true);
        }

        private Task OnCharacterPickOk()
        {
            return gameProtocol.RunInMainThread(() =>
            {
                gameObject.SetActive(false);
            });
        }

        private Task OnCharacterPickError(CharacterPickError error)
        {
            return gameProtocol.RunInMainThread(() =>
            {
                string text;
                switch (error.Code)
                {
                    case CharacterPickError.CharacterPickErrorCode.AlreadyPicked:
                        text = "A character is already picked";
                        break;
                    case CharacterPickError.CharacterPickErrorCode.InvalidIndex:
                        text = "Invalid character";
                        break;
                    default:
                        text = "Unknown error";
                        break;
                }
                statusText.text = "Error picking character: " + text;
            });
        }

        private Task OnCharacterListOk(CharactersNamesList list)
        {
            // This implementation assumes no paging, but a list of
            // all the characters is rendered.

            return gameProtocol.RunInMainThread(() =>
            {
                if (list.CharacterNames.Length == 0)
                {
                    emptyPane.SetActive(true);
                    buttonsPane.SetActive(false);
                    statusText.text = "Create a character to continue...";
                }
                else
                {
                    emptyPane.SetActive(false);
                    buttonsPane.SetActive(true);
                    statusText.text = "Pick a character to continue...";
                    for (int idx = 0; idx < buttons.Length; idx++)
                    {
                        Button button = buttons[idx];
                        if (idx < list.CharacterNames.Length)
                        {
                            int idx2 = idx;
                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(() => { PickCharacter(idx2); });
                            button.GetComponent<TMP_Text>().text = list.CharacterNames[idx];
                            button.gameObject.SetActive(true);
                        }
                        else
                        {
                            button.gameObject.SetActive(false);
                        }
                    }
                }
            });
        }

        private Task OnCharacterListError()
        {
            return gameProtocol.RunInMainThread(() =>
            {
                statusText.text = "There was an error retrieving the characters";
            });
        }

        private async void PickCharacter(int index)
        {
            statusText.text = "Piking character...";
            await gameProtocol.CharacterPick((uint)index);
        }
    }
}