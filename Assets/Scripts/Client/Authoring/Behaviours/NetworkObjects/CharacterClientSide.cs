using System;
using UnityEngine;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Client;
using Core.Authoring.Behaviours.MapObjects;
using Models;

namespace Client.Authoring.Behaviours.NetworkObjects
{
    [RequireComponent(typeof(Character))]
    public class CharacterClientSide : OwnedNetRoseModelClientSide<CharacterSpawnData, CharacterRefreshData>
    {
        /// <summary>
        ///   Tracks the current instance.
        /// </summary>
        public static CharacterClientSide Instance { get; private set; } = null;

        /// <summary>
        ///   An event to attend when the local instance is spawned.
        /// </summary>
        public static event Action OnPlayerCharacterSpawned;
        
        /// <summary>
        ///   An event to attend when the local instance is de-spawned.
        /// </summary>
        public static event Action OnPlayerCharacterDespawned;
        
        // The main camera.
        private static Camera mainCamera;

        // The character model.
        private Character character;
        
        protected override void Awake()
        {
            base.Awake();
            if (mainCamera == null) mainCamera = Camera.main;
            OnSpawned += NetRoseModelClientSide_OnSpawned;
            OnDespawned += NetRoseModelClientSide_OnDespawned;
            character = GetComponent<Character>();
        }
        
        protected override void OnDestroy()
        {
            OnSpawned -= NetRoseModelClientSide_OnSpawned;
            OnDespawned -= NetRoseModelClientSide_OnDespawned;
            base.OnDestroy();
        }

        private void NetRoseModelClientSide_OnSpawned()
        {
            // Do something on local spawn.
        }

        private void NetRoseModelClientSide_OnDespawned()
        {
            // Do something on local despawn.
            if (Instance == this)
            {
                Instance = null;
                OnPlayerCharacterDespawned?.Invoke();
            }
        }

        protected override void InflateOwnedFrom(CharacterSpawnData fullData)
        {
            if (isOwned)
            {
                Instance = this;
                OnPlayerCharacterSpawned?.Invoke();
            }
            character.Name = fullData.DisplayName;
            fullData.ApplyInto(MapObject);
        }
        
        protected override void UpdateFrom(CharacterRefreshData refreshData)
        {
            if (refreshData.Text != null) character.SetText(refreshData.Text);
            refreshData.ApplyInto(MapObject);
        }
        
        private void Update()
        {
            if (Instance == this) mainCamera.transform.position = new Vector3(
                Instance.transform.position.x,
                Instance.transform.position.y,
                mainCamera.transform.position.z
            );
        }
    }
}
