using System;
using System.Threading;
using System.Threading.Tasks;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Server;
using AlephVault.Unity.NetRose.Types.Models;
using Core.Types.Characters;
using Models;
using Server.Authoring.Behaviours.External.Models;

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
        
        protected void Awake()
        {
            base.Awake();
            ownedSpawnData = new OwnedModel<CharacterSpawnData>(true, spawnData);
            notOwnedSpawnData = new OwnedModel<CharacterSpawnData>(false, spawnData);
        }
        
        protected override OwnedModel<CharacterSpawnData> GetInnerFullData(ulong connectionId)
        {
            return GetOwner() == connectionId ? ownedSpawnData : notOwnedSpawnData;
        }

        protected override CharacterRefreshData GetInnerRefreshData(ulong connectionId, string context)
        {
            return refreshData;
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

        // Updates the proper refresh data of the object.
        private Task RefreshWith(CharacterRefreshData data)
        {
            Mutex m = new();
            try
            {
                refreshData = data;
                return Refresh();
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
        public Task Say(string text)
        {
            return RefreshWith(new CharacterRefreshData {
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
