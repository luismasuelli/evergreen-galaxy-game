using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.WindRose.Authoring.Behaviours.World;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using Protocols;
using Protocols.Messages;

namespace Server.Authoring.Behaviours.Protocols
{
    [RequireComponent(typeof(PlayerProtocolServerSide))]
    // Uncomment this line if this class should use throttling.
    // [RequireComponent(typeof(ServerSideThrottler))]
    public class MainProtocolServerSide : ProtocolServerSide<MainProtocolDefinition>
    {
        // Define variables to hold senders, one for each defined
        // client message. For this generated boilerplate examples,
        // as per the MainProtocolDefinition class, you'll
        // need these examples. Work analogous to create your own
        // messages and their senders.
        //
        // private Func<ulong, Task> SendIntroduceYourself;
        // private Func<ulong, MyType, Task> SendSomeTypedMessage;
        //
        // NOTES: These function references can be invoked ANYWHERE,
        // not just in the context of the incoming message handlers
        // that are defined below, as long as the protocol is ready.
        //
        // If it is needed to have a sender that works for multiple
        // connections simultaneously, declare a BROADCASTER instead:
        //
        // private Func<IEnumerable<ulong>, Dictionary<ulong, Task>> BroadcastHello;
        // private Func<IEnumerable<ulong>, MyType, Dictionary<ulong, Task>> BroadcastSomeTypedMessage;

        private PlayerProtocolServerSide principalProtocol;
        // Uncomment these lines if this class should use throttling.
        // Also, ensure the throttler has at least 3 throttling indices.
        //
        // private ServerSideThrottler throttler;
        // private const int WalkThrottle = 0;
        // private const int SimpleCommandThrottle = 1;
        // private const int AimedCommandThrottle = 2;
                
        /// <summary>
        ///   A Post-Awake hook.
        /// </summary>
        protected override void Setup()
        {
            principalProtocol = GetComponent<PlayerProtocolServerSide>();
            // Uncomment this line if this class should use throttling.
            // throttler = GetComponent<ServerSideThrottler>();
        }
        
        /// <summary>
        ///   Initializes the protocol senders & broadcasters once
        ///   the server is ready.
        /// </summary>
        protected override void Initialize()
        {
            // This is the place to assign the initialization of
            // these message senders by calling MakeSender or
            // MakeSender<T>, depending on how the message was
            // defined in the protocol.
            //
            // SendSomeTypedMessage = MakeSender<MyType2>("SomeTypedClientMessage");
            // SendHello = MakeSender("Hello");
            //
            // You can also assign this error handler:
            // OnSendError = async (exc) { ... process an exception ... };
            //
            // That one will allow you to wrap any message send:
            //
            // await UntilSendIsDone(SendHello(someClientId));
            // await UntilSendIsDone(SendSomeTypedMessage(someClientId, new MyType2(...)));
            //
            // Which will capture any error by calling OnSendError(e).
        }
        
        /// <summary>
        ///   Initializes the protocol handlers once the server is ready.
        /// </summary>
        protected override void SetIncomingMessageHandlers()
        {
            // Typically, one of these games involves the ability
            // to move in any of the 4 directions:
            AddIncomingMessageHandler(MainProtocolDefinition.MoveDown, async (proto, connId) => {
                // Uncomment the //-commented lines if using throttle:
                //
                // await throttler.DoThrottled(clientId, async () => {
                    try
                    {
                        principalProtocol.MoveDown(connId, true);
                    }
                    catch(Exception e) { /* Handle this */ }
                // }, (ulong clientId, DateTime when, int throttlesCount) => { /* handle this */ }, WalkThrottle);
            });
            AddIncomingMessageHandler(MainProtocolDefinition.MoveLeft, async (proto, connId) => {
                // Uncomment the //-commented lines if using throttle:
                //
                // await throttler.DoThrottled(clientId, async () => {
                    try
                    {
                        principalProtocol.MoveLeft(connId, true);
                    }
                    catch(Exception e) { /* Handle this */ }
                // }, (ulong clientId, DateTime when, int throttlesCount) => { /* handle this */ }, WalkThrottle);
            });
            AddIncomingMessageHandler(MainProtocolDefinition.MoveRight, async (proto, connId) => {
                // Uncomment the //-commented lines if using throttle:
                //
                // await throttler.DoThrottled(clientId, async () => {
                    try
                    {
                        principalProtocol.MoveRight(connId, true);
                    }
                    catch(Exception e) { /* Handle this */ }
                // }, (ulong clientId, DateTime when, int throttlesCount) => { /* handle this */ }, WalkThrottle);
            });
            AddIncomingMessageHandler(MainProtocolDefinition.MoveUp, async (proto, connId) => {
                // Uncomment the //-commented lines if using throttle:
                //
                // await throttler.DoThrottled(clientId, async () => {
                    try
                    {
                        principalProtocol.MoveUp(connId, true);
                    }
                    catch(Exception e) { /* Handle this */ }
                // }, (ulong clientId, DateTime when, int throttlesCount) => { /* handle this */ }, WalkThrottle);
            });
            AddIncomingMessageHandler(MainProtocolDefinition.SomeSimpleCommand, async (proto, connId) => {
                // Uncomment the //-commented lines if using throttle:
                //
                // await throttler.DoThrottled(clientId, async () => {
                    try
                    {
                        Map map = principalProtocol.GetPrincipal(connId).MapObject.ParentMap;
                        if (!map) return;
                        /* Do something here */

                        /** Example: compute closest (x, y) for object
                            according to its orientation and do like
                            this:
                        MapObject target = GetExactTarget(map, newX, newY);
                        if (target) {
                            // Something else here.
                        }
                        */
                    }
                    catch(Exception e) { /* Handle this */ }
                // }, (ulong clientId, DateTime when, int throttlesCount) => { /* handle this */ }, SimpleCommandThrottle);                
            });
            AddIncomingMessageHandler<AimType>(MainProtocolDefinition.SomeAimedCommand, async (proto, connId, aim) => {
                // Uncomment the //-commented lines if using throttle:
                //
                // await throttler.DoThrottled(clientId, async () => {
                    try
                    {
                        Map map = principalProtocol.GetPrincipal(connId).MapObject.ParentMap;
                        if (!map) return;
                        Scope scope = map.ParentScope;
                        if (!scope) return;
                        Map newMap = scope[aim.Map];
                        MapObject target = GetLowestTarget(newMap, aim.X, aim.Y);
                        if (target) {
                            /* Do something with the target */
                        }
                    }
                    catch(Exception e) { /* Handle this */ }
                // }, (ulong clientId, DateTime when, int throttlesCount) => { /* handle this */ }, AimedCommandThrottle);                
            });
        }
        
        private MapObject GetLowestTarget(Map map, ushort x, ushort y)
        {
            // Ensure all your maps use spatial indices for this to match something.
            IEnumerable<MapObject> objs = map.ObjectsLayer.StrategyHolder.Search(x, y);
            MapObject lowestObj = null;
            foreach(MapObject obj in objs)
            {
                if (lowestObj == null || obj.Y < lowestObj.Y) lowestObj = obj;
            }
            return lowestObj;
        }

        private MapObject GetExactTarget(Map map, ushort x, ushort y)
        {
            // Ensure all your maps use spatial indices for this to match something.
            IEnumerable<MapObject> objs = map.ObjectsLayer.StrategyHolder.Search(x, y);
            foreach(MapObject obj in objs)
            {
                if (obj.X == x && obj.Y == y) return obj;
            }
            return null;
        }
        
        /// <summary>
        ///   Handler for when a client connection is established.
        /// </summary>
        public override async Task OnConnected(ulong clientId)
        {
            // This method is optional. It can be removed.
            //
            // Notice how the connections are identified by some sort
            // of clientId. Typically, some set of custom dictionaries
            // are used to track the life of client connections.
            //
            // If you need to interact with Unity components,
            // use RunInMainThread in the same way it is told
            // in this example inside the incoming message
            // handlers.
            
            // Uncomment this line if your class should use throttling.
            // throttler.TrackConnection(clientId);
        }

        /// <summary>
        ///   Handler for when a client connection is terminated,
        ///   be it graceful or due to an error.
        /// </summary>
        public override async Task OnDisconnected(ulong clientId, Exception reason)
        {
            // This method is optional. It can be removed.
            //
            // Only per-client setup should be done. Do not
            // attempt to send any message to that client: it
            // has already disconnected.
            //
            // If you need to interact with Unity components,
            // use RunInMainThread in the same way it is told
            // in this example inside the incoming message
            // handlers.

            // Uncomment this line if your class should use throttling.
            // throttler.UntrackConnection(clientId);
        }
        
        /// <summary>
        ///   Handler for when the server is successfully started.
        /// </summary>
        public override async Task OnServerStarted()
        {
            // This method is optional. It can be removed.
            //
            // Only setup operations should be done here.
            //
            // If you need to interact with Unity components,
            // use RunInMainThread in the same way it is told
            // in this example inside the incoming message
            // handlers.

            // Uncomment this line if your class should use throttling.
            // throttler.Startup();
        }

        /// <summary>
        ///   Handler for when the server is shutdown, be it graceful or due
        ///   to an error.
        /// </summary>
        public override async Task OnServerStopped(Exception e)
        {
            // This method is optional. It can be removed.
            //
            // Only cleanup operations should be done here.
            // Do not attempt to send any message here, since
            // any attempt will fail (and should be considered
            // unstable and unsafe if, for some reason, any
            // connection still remains).
            //
            // If you need to interact with Unity components,
            // use RunInMainThread in the same way it is told
            // in this example inside the incoming message
            // handlers.

            // Uncomment this line if your class should use throttling.
            // throttler.Teardown();
        }
    }
}