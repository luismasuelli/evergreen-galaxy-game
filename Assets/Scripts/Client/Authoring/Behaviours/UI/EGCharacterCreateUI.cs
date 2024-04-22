using System;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using Core.Types.Characters;
using Protocols.Messages;
using TMPro;
using UnityEngine;

namespace Client.Authoring.Behaviours.UI
{
    using Protocols;
    using UnityEngine.UI;

    /// <summary>
    ///   A UI to pick a character.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class EGCharacterCreateUI : MonoBehaviour
    {
        /// <summary>
        ///   The involved network client.
        /// </summary>
        [SerializeField]
        private NetworkClient client;
        
        // The game protocol.
        private EGGameProtocolClientSide gameProtocol;

        /// <summary>
        ///   The character pick UI.
        /// </summary>
        [SerializeField]
        private GameObject characterPickUI;
        
        /// <summary>
        ///   The involved "Pick Character" button.
        /// </summary>
        [SerializeField]
        private Button goCharacterPickButton;

        /// <summary>
        ///   The involved "Display Name" field.
        /// </summary>
        [SerializeField]
        private TMP_InputField displayNameField;

        /// <summary>
        ///   The involved "Sex" field.
        /// </summary>
        [SerializeField]
        private TMP_Dropdown sexField;
        
        /// <summary>
        ///   The involved "Race" field.
        /// </summary>
        [SerializeField]
        private TMP_Dropdown raceField;
        
        /// <summary>
        ///   The involved "Hair" field.
        /// </summary>
        [SerializeField]
        private TMP_Dropdown hairField;
        
        /// <summary>
        ///   The involved "HairColor" field.
        /// </summary>
        [SerializeField]
        private TMP_Dropdown hairColorField;

        /// <summary>
        ///   The "Submit" button.
        /// </summary>
        [SerializeField]
        private Button submitButton;
        
        /// <summary>
        ///   The status text.
        /// </summary>
        [SerializeField]
        private TMP_Text statusText;

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

            if (!characterPickUI)
            {
                throw new Exception("No character pick UI is set");
            }
            if (!goCharacterPickButton)
            {
                throw new Exception("No 'Pick Character' button is set");
            }

            if (!displayNameField)
            {
                throw new Exception("No 'Display Name' field is set");
            }

            if (!sexField)
            {
                throw new Exception("No 'Sex' field is set");
            }

            if (!raceField)
            {
                throw new Exception("No 'Race' field is set");
            }

            if (!hairField)
            {
                throw new Exception("No 'Hair' field is set");
            }

            if (!hairColorField)
            {
                throw new Exception("No 'Hair Color' field is set");
            }

            if (!submitButton)
            {
                throw new Exception("No 'Submit' button is set");
            }

            if (!statusText)
            {
                throw new Exception("No status text is set");
            }
        }

        private void Start()
        {
            gameProtocol.OnCharacterCreated += OnCharacterCreated;
            gameProtocol.OnCharacterCreateError += OnCharacterCreateError;
            submitButton.onClick.AddListener(CreateCharacter);
            goCharacterPickButton.onClick.AddListener(GoCharacterPick);
        }

        private void OnDestroy()
        {
            gameProtocol.OnCharacterCreated -= OnCharacterCreated;
            gameProtocol.OnCharacterCreateError -= OnCharacterCreateError;
            submitButton.onClick.RemoveListener(CreateCharacter);
            goCharacterPickButton.onClick.RemoveListener(GoCharacterPick);
        }

        private void GoCharacterPick()
        {
            gameObject.SetActive(false);
            characterPickUI.SetActive(true);
        }

        private void CreateCharacter()
        {
            gameProtocol.CharacterCreate(new CharacterCreationData
            {
                DisplayName = displayNameField.text,
                Sex = (SexType)sexField.value,
                Race = (RaceType)raceField.value,
                Hair = (HairType)hairField.value,
                HairColor = (HairColorType)hairColorField.value,
            });
        }

        private Task OnCharacterCreated()
        {
            // Anyway, this UI will cease to be visible.
            return gameProtocol.RunInMainThread(() =>
            {
                gameObject.SetActive(false);
                characterPickUI.SetActive(true);
                displayNameField.text = "";
                raceField.value = 0;
                sexField.value = 0;
                hairField.value = 0;
                hairColorField.value = 0;
            });
        }
        
        private Task OnCharacterCreateError(CharacterCreateError error)
        {
            return gameProtocol.RunInMainThread(() =>
            {
                string text;
                switch (error.Code)
                {
                    case CharacterCreateError.CharacterCreateErrorCode.AlreadyPicked:
                        text = "A character is already picked";
                        break;
                    case CharacterCreateError.CharacterCreateErrorCode.InvalidData:
                        text = "Invalid data";
                        break;
                    case CharacterCreateError.CharacterCreateErrorCode.MaxCharactersReached:
                        text = "You cannot create more characters";
                        break;
                    case CharacterCreateError.CharacterCreateErrorCode.DisplayNameInUse:
                        text = "Character name in use";
                        break;
                    default:
                        text = "Unknown error";
                        break;
                }
                statusText.text = "Error picking character: " + text;
            });
        }
    }
}