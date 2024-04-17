using System;
using System.Threading;
using System.Threading.Tasks;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Server;
using AlephVault.Unity.NetRose.Types.Models;
using Core.Types.Characters;
using Models;
using Server.Authoring.Behaviours.External.Models;
using Position = Server.Authoring.Behaviours.External.Models.Position;

namespace Server.Authoring.Behaviours.NetworkObjects
{
    public class CharacterServerSide : OwnedNetRoseModelServerSide<CharacterSpawnData, CharacterRefreshData>
    {
        // The external object holding the character data.
        private Character characterData;
        private CharacterSpawnData spawnData = new();
        private OwnedModel<CharacterSpawnData> ownedSpawnData;
        private OwnedModel<CharacterSpawnData> notOwnedSpawnData;
        private CharacterRefreshData refreshData;
        private bool isSpawned = false;
        
        protected void Awake()
        {
            base.Awake();
            ownedSpawnData = new OwnedModel<CharacterSpawnData>(true, spawnData);
            notOwnedSpawnData = new OwnedModel<CharacterSpawnData>(false, spawnData);
            OnAfterSpawned += HandleOnAfterSpawned;
            OnBeforeDespawned += HandleOnBeforeDeSpawned;
        }

        protected void OnDestroy()
        {
            OnAfterSpawned -= HandleOnAfterSpawned;
            OnBeforeDespawned -= HandleOnBeforeDeSpawned;
        }
        
        protected override OwnedModel<CharacterSpawnData> GetInnerFullData(ulong connectionId)
        {
            return GetOwner() == connectionId ? ownedSpawnData : notOwnedSpawnData;
        }

        protected override CharacterRefreshData GetInnerRefreshData(ulong connectionId, string context)
        {
            return refreshData;
        }

        private async Task HandleOnAfterSpawned()
        {
            isSpawned = true;
        }

        private async Task HandleOnBeforeDeSpawned()
        {
            isSpawned = false;
        }

        /// <summary>
        ///   Sets the whole character data.
        /// </summary>
        /// <param name="character">The character data</param>
        public void Initialize(Character character)
        {
            characterData = character;
            spawnData.DisplayName = character.DisplayName;
            spawnData.HairValue = character.Hair;
            spawnData.HairColorValue = character.HairColor;
            spawnData.RaceValue = character.Race;
            spawnData.SexValue = character.Sex;
            spawnData.ClothColorValue = character.ClothColor;
        }

        /// <summary>
        ///   Saves the whole character data.
        /// </summary>
        public void Save()
        {
            characterData.DisplayName = spawnData.DisplayName;
            characterData.Hair = spawnData.HairValue;
            characterData.HairColor = spawnData.HairColorValue;
            characterData.Race = spawnData.RaceValue;
            characterData.Sex = spawnData.SexValue;
            characterData.ClothColor = characterData.ClothColor;
            characterData.Position = new Position
            {
                Map = (ushort)(MapObject.ParentMap?.GetIndex() ?? 0),
                Scope = NetRoseScopeServerSide?.DefaultName ?? "",
                X = MapObject.X,
                Y = MapObject.Y
            };
        }

        // Updates the proper refresh data of the object.
        private async Task RefreshWith(CharacterRefreshData data)
        {
            var m = new Mutex();
            m.WaitOne();
            try
            {
                refreshData = data;
                await Refresh();
            }
            finally
            {
                m.ReleaseMutex();
            }
        }

        /// <summary>
        ///   Forces the character to say something.
        /// </summary>
        /// <param name="text">The text to say</param>
        public void Say(string text)
        {
            RefreshWith(new CharacterRefreshData {
                Text = text ?? ""
            });
        }

        /// <summary>
        ///   Sets the cloth color.
        /// </summary>
        public ClothColorType ClothColor
        {
            get => characterData.ClothColor;
            set
            {
                characterData.ClothColor = value;
                spawnData.ClothColorValue = value;
                RefreshWith(new CharacterRefreshData {
                    ClothColorValue = value
                });
            }
        }

        /// <summary>
        ///   Rotates the cloth color.
        /// </summary>
        public void RotateClothColor()
        {
            ClothColor = (ClothColorType)(((int)ClothColor + 1) % 6);
        }
    }
}
