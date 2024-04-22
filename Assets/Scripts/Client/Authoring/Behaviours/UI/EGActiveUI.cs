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
        ///   The dialog to select a character.
        /// </summary>
        [SerializeField]
        private GameObject selectCharacterDialog;

        /// <summary>
        ///   The dialog to create a character.
        /// </summary>
        [SerializeField]
        private GameObject createCharacterDialog;

        // The player protocol.
        private NetRoseProtocolClientSide playerProtocol;

        private void Awake()
        {
            if (!client)
            {
                throw new Exception("No network client is referenced in this object!");
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
            CharacterClientSide.OnPlayerCharacterSpawned += OnPlayerCharacterSpawned;
        }
        
        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnCloseClick);
            CharacterClientSide.OnPlayerCharacterSpawned -= OnPlayerCharacterSpawned;
        }
        
        private void OnEnable()
        {
            closeDialog.SetActive(false);
            selectCharacterDialog.SetActive(true);
        }
        
        private void OnPlayerCharacterSpawned()
        {
            playerProtocol.RunInMainThread(() =>
            {
                closeDialog.SetActive(false);
                selectCharacterDialog.SetActive(false);
                createCharacterDialog.SetActive(false);
            });
        }
        
        private void OnCloseClick()
        {
            closeDialog.SetActive(true);
        }
    }
}
