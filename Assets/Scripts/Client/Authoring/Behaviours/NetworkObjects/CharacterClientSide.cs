// Using UnityEngine;
using AlephVault.Unity.Meetgard.Types;
using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Client;
using AlephVault.Unity.NetRose.Types.Models;
using Models;

namespace Client.Authoring.Behaviours.NetworkObjects
{
    public class CharacterClientSide : OwnedNetRoseModelClientSide<CharacterSpawnData, CharacterRefreshData>
    {
        /// <summary>
        ///   Tracks the current instance.
        /// </summary>
        public static CharacterClientSide Instance { get; private set; } = null;
        
        // THIS BEHAVIOUR IS MEANT TO BE ATTACHED TO MAP OBJECTS
        // THAT ARE MEANT TO BE SYNCHRONIZED THROUGH THE NETWORK.
        // THIS MEANS: OBJECTS THAT SHOULD RECEIVE REAL-TIME UPDATES
        // FROM THE CONNECTED SERVER.
                
        protected override void Awake()
        {
            base.Awake();
            OnSpawned += NetRoseModelClientSide_OnSpawned;
            OnDespawned += NetRoseModelClientSide_OnDespawned;
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
            if (Instance == this) Instance = null;
        }

        protected override void InflateOwnedFrom(CharacterSpawnData fullData)
        {
            // Implement this method to visually update this object
            // based on the full spawn data received through the
            // network update.
            if (isOwned) Instance = this;
        }
        
        protected override void UpdateFrom(CharacterRefreshData refreshData)
        {
            // Implement this method to visually update this object
            // based on the refresh data received through the network
            // update. Depending on how the game is designed, it can
            // represent a full refresh data object or a partial one,
            // so some aspects are only meant to be refreshed.
        }
        
        /**
        private void Update()
        {
            if (CharacterClientSide.Instance) Camera.main.transform.position = new Vector3(
                CharacterClientSide.Instance.transform.position.x,
                CharacterClientSide.Instance.transform.position.y,
                Camera.main.transform.position.z
            );
        }
        */
    }
}
