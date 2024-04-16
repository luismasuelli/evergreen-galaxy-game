using System;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.WindRose.Authoring.Behaviours.World;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Client;
using Protocols;
using Protocols.Messages;

namespace Client.Authoring.Behaviours.Protocols
{
    [RequireComponent(typeof(EGAuthProtocolClientSide))]
    [RequireComponent(typeof(ClientSideThrottler))]
    public class EGGameProtocolClientSide : ProtocolClientSide<EGGameProtocolDefinition>
    {
        private ClientSideThrottler throttler;
        private EGAuthProtocolClientSide authProtocol;
        private const int WalkThrottle = 0;

        // Typically, one of these games involves the ability
        // to move in any of the 4 directions:
        private Func<Task> SendMoveDown;
        private Func<Task> SendMoveLeft;
        private Func<Task> SendMoveRight;
        private Func<Task> SendMoveUp;
        
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
            SendMoveDown = throttler.MakeThrottledSender(SendMoveDown);
            SendMoveLeft = throttler.MakeThrottledSender(SendMoveLeft);
            SendMoveRight = throttler.MakeThrottledSender(SendMoveRight);
            SendMoveUp = throttler.MakeThrottledSender(SendMoveUp);
        }

        private Task DoSendMoveDown()
        {
            return authProtocol.LoggedIn ? SendMoveDown() : Task.CompletedTask;
        }
        
        private Task DoSendMoveUp()
        {
            return authProtocol.LoggedIn ? SendMoveUp() : Task.CompletedTask;
        }
        
        private Task DoSendMoveLeft()
        {
            return authProtocol.LoggedIn ? SendMoveLeft() : Task.CompletedTask;
        }
        
        private Task DoSendMoveRight()
        {
            return authProtocol.LoggedIn ? SendMoveRight() : Task.CompletedTask;
        }
        
        /// <summary>
        ///   Initializes the protocol handlers once the server is ready.
        /// </summary>
        protected override void SetIncomingMessageHandlers()
        {
            // This is the place to assign handlers to incoming messages.
            // Since messages can be typed or untyped, as in the examples
            // that were generated, there are two flavors for the message
            // handling definition: typed, and untyped.
            //
            // AddIncomingMessageHandler("IntroduceYourself", async (proto) => {
            //     // Notice how this message is not typed. The only argument
            //     // is the protocol client side object itself.
            //     //
            //     // You can do what you want here, including sending messages:
            //     // _ = SendHello(); // or: await SendHello();
            //     // The difference is that, by awaiting, we ensure the message
            //     // was actually sent or an error was triggered.
            //     //
            //     // PLEASE NOTE: IF YOUR CODE INVOLVES INTERACTION WITH UNITY
            //     // COMPONENTS, THIS MUST ONLY OCCUR IN THE MAIN THREAD, and
            //     // these handlers DO NOT RUN IN THE MAIN THREAD. You can do
            //     // it by calling:
            //     //
            //     // await RunInMainThread(async () { ... the code ... });...
            //     //
            //     // Or, if waiting for it is not needed, just:
            //     //
            //     // _ = RunInMainThread(async () { ... the code ... });            
            // });
            //
            // AddIncomingMessageHandler<MyType>("SomeTypedServerMessage", async (proto, msg) => {
            //     // Notice how this message IS typed, as defined in the
            //     // protocol definition. There is a new `msg` argument.
            //     // This argument is of type MyType.
            //     //
            //     // Otherwise, this is the same as the untyped case.
            // });
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