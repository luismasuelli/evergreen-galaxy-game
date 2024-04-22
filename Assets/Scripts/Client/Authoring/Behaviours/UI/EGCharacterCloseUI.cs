using System;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using UnityEngine;

namespace Client.Authoring.Behaviours.UI
{
    using Protocols;
    using UnityEngine.UI;

    /// <summary>
    ///   A UI to close.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class EGCharacterCloseUI : MonoBehaviour
    {
        /// <summary>
        ///   The involved network client.
        /// </summary>
        [SerializeField]
        private NetworkClient client;

        // The auth protocol.
        private EGAuthProtocolClientSide authProtocol;
        
        // The game protocol.
        private EGGameProtocolClientSide gameProtocol;

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

        private void Awake()
        {
            if (!client)
            {
                throw new Exception("No network client is referenced in this object!");
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
        }

        private void Start()
        {
            closeLogoutButton.onClick.AddListener(OnCloseLogoutClick);
            closeLobbyButton.onClick.AddListener(OnCloseLobbyClick);
            closeNoButton.onClick.AddListener(OnCloseNoClick);
        }

        private void OnDestroy()
        {
            closeLogoutButton.onClick.RemoveListener(OnCloseLogoutClick);
            closeLobbyButton.onClick.RemoveListener(OnCloseLobbyClick);
            closeNoButton.onClick.RemoveListener(OnCloseNoClick);
        }
        
        private void OnCloseLogoutClick()
        {
            _ = authProtocol.Logout();
            gameObject.SetActive(false);
        }

        private void OnCloseLobbyClick()
        {
            _ = gameProtocol.CharacterRelease();
            gameObject.SetActive(false);
        }

        private void OnCloseNoClick()
        {
            gameObject.SetActive(false);
        }
    }
}