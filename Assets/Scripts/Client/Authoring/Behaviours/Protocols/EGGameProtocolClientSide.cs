using System;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.WindRose.Authoring.Behaviours.World;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Client;
using AlephVault.Unity.Support.Utils;
using Protocols;
using Protocols.Messages;

namespace Client.Authoring.Behaviours.Protocols
{
    using String = AlephVault.Unity.Binary.Wrappers.String;

    [RequireComponent(typeof(EGAuthProtocolClientSide))]
    [RequireComponent(typeof(ClientSideThrottler))]
    public class EGGameProtocolClientSide : ProtocolClientSide<EGGameProtocolDefinition>
    {
        private ClientSideThrottler throttler;
        private EGAuthProtocolClientSide authProtocol;
        private const int WalkThrottle = 0;
        private const int SayThrottle = 1;
        private const int ClothThrottle = 2;
        private const int CharacterCommandThrottle = 3;

        // Typically, one of these games involves the ability
        // to move in any of the 4 directions:
        private Func<Task> SendMoveDown;
        private Func<Task> SendMoveLeft;
        private Func<Task> SendMoveRight;
        private Func<Task> SendMoveUp;
        private Func<Task> SendClothRotate;
        private Func<String, Task> SendSay;
        private Func<Task> SendCharacterList;
        
        /// <summary>
        ///   A Post-Awake hook.
        /// </summary>
        protected override void Setup()
        {
            throttler = GetComponent<ClientSideThrottler>();
            authProtocol = GetComponent<EGAuthProtocolClientSide>();
        }
        
        /// <summary>
        ///   Initializes the protocol senders once the server is ready.
        /// </summary>
        protected override void Initialize()
        {
            // Typically, one of these games involves the ability
            // to move in any of the 4 directions:
            SendMoveDown = MakeSender(EGGameProtocolDefinition.MoveDown);
            SendMoveLeft = MakeSender(EGGameProtocolDefinition.MoveLeft);
            SendMoveRight = MakeSender(EGGameProtocolDefinition.MoveRight);
            SendMoveUp = MakeSender(EGGameProtocolDefinition.MoveUp);
            SendClothRotate = MakeSender(EGGameProtocolDefinition.ClothRotate);
            SendSay = MakeSender<String>(EGGameProtocolDefinition.Say);
            SendCharacterList = MakeSender(EGGameProtocolDefinition.CharacterList);
            SendMoveDown = throttler.MakeThrottledSender(SendMoveDown, WalkThrottle);
            SendMoveLeft = throttler.MakeThrottledSender(SendMoveLeft, WalkThrottle);
            SendMoveRight = throttler.MakeThrottledSender(SendMoveRight, WalkThrottle);
            SendMoveUp = throttler.MakeThrottledSender(SendMoveUp, WalkThrottle);
            SendClothRotate = throttler.MakeThrottledSender(SendClothRotate, ClothThrottle);
            SendSay = throttler.MakeThrottledSender(SendSay, SayThrottle);
            SendCharacterList = throttler.MakeThrottledSender(SendCharacterList, CharacterCommandThrottle);
        }

        public Task MoveDown()
        {
            return authProtocol.LoggedIn ? SendMoveDown() : Task.CompletedTask;
        }
        
        public Task MoveUp()
        {
            return authProtocol.LoggedIn ? SendMoveUp() : Task.CompletedTask;
        }
        
        public Task MoveLeft()
        {
            return authProtocol.LoggedIn ? SendMoveLeft() : Task.CompletedTask;
        }
        
        public Task MoveRight()
        {
            return authProtocol.LoggedIn ? SendMoveRight() : Task.CompletedTask;
        }

        public Task Say(string message)
        {
            // Yes, trimming before and after.
            message = message.Trim().Substring(0, EGGameProtocolDefinition.MaxSayLength).Trim();
            return authProtocol.LoggedIn ? SendSay((String)message) : Task.CompletedTask;
        }

        public Task ClothRotate()
        {
            return authProtocol.LoggedIn ? SendClothRotate() : Task.CompletedTask;
        }

        public Task CharacterList()
        {
            return authProtocol.LoggedIn ? SendCharacterList() : Task.CompletedTask;
        }

        public event Func<CharactersNamesList, Task> OnCharacterListContent = null;
        public event Func<Task> OnCharacterListError = null;
        
        /// <summary>
        ///   Initializes the protocol handlers once the server is ready.
        /// </summary>
        protected override void SetIncomingMessageHandlers()
        {
            AddIncomingMessageHandler<CharactersNamesList>(EGGameProtocolDefinition.CharacterListContent,
                async (_, content) =>
                {
                    await (OnCharacterListContent?.InvokeAsync(content) ?? Task.CompletedTask);
                });
            AddIncomingMessageHandler(EGGameProtocolDefinition.CharacterListError,
                async (_) =>
                {
                    await (OnCharacterListError?.InvokeAsync() ?? Task.CompletedTask);
                });
        }
        
        private AimType GetAimCell(INetRoseModelClientSide obj, Camera camera, Vector3 mousePosition)
        {
            // Use this function to generate a record of AimType class.
            
            // Alternatively, use this implementation:
            // 1. Reach the map's ParentScope.
            // 2. Iterate over the maps (idx=0 to Count-1).
            // 3. Try converting WorldToCell for each iterated map.
            // 4. If the X coordinate is valid in 0..map.Width-1 and
            //    the Y coordinate is valid in 0..map.Height-1 or perhaps
            //    in 0..map.Height (this one would be allowed in games
            //    like Argentum Online) then return that one instead.

            if (obj == null) return null;
            Map map = obj.MapObject.ParentMap;
            if (map == null) return null;
            int index = map.ParentScope == null ? 0 : map.GetIndex();
            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);
            Vector3 cell = map.ObjectsLayer.GetComponent<Grid>().WorldToCell(worldPosition);
            return new AimType { Map = index, X = (ushort)cell.x, Y = (ushort)cell.x };
        }
        
        /// <summary>
        ///   Handler for when the connection is successfully established.
        /// </summary>
        public override async Task OnConnected()
        {
            // Do what you want, including sending messages.
            // If you need to interact with Unity components,
            // use RunInMainThread in the same way it is told
            // in this example inside the incoming message
            // handlers.
        }

        /// <summary>
        ///   Handler for when the connection is terminated, be it graceful
        ///   or due to an error.
        /// </summary>
        public override async Task OnDisconnected(Exception reason)
        {
            // The server was just disconnected. Do everything
            // except attempting to send messages or access the
            // connection somehow, since it was already ended.
            //
            // If you need to interact with Unity components,
            // use RunInMainThread in the same way it is told
            // in this example inside the incoming message
            // handlers.
        }
    }
}