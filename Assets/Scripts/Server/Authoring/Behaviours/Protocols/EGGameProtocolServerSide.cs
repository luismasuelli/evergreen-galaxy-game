using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlephVault.Unity.Binary;
using AlephVault.Unity.Binary.Wrappers;
using UnityEngine;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Protocols;
using Protocols.Messages;
using Server.Authoring.Behaviours.External.Models;
using Server.Authoring.Behaviours.NetworkObjects;

namespace Server.Authoring.Behaviours.Protocols
{
    using String = AlephVault.Unity.Binary.Wrappers.String;
    using Map = AlephVault.Unity.WindRose.Authoring.Behaviours.World.Map;

    [RequireComponent(typeof(PlayerProtocolServerSide))]
    [RequireComponent(typeof(EGAuthProtocolServerSide))]
    [RequireComponent(typeof(ServerSideThrottler))]
    public class EGGameProtocolServerSide : ProtocolServerSide<EGGameProtocolDefinition>
    {
        private const int MaxCharacters = 10;
        private PlayerProtocolServerSide principalProtocol;
        private EGAuthProtocolServerSide authProtocol;
        private ServerSideThrottler throttler;
        private const int WalkThrottle = 0;
        private const int SayThrottle = 1;
        private const int ClothThrottle = 2;
        private const int CharacterCommandThrottle = 3;

        private Func<ulong, CharactersNamesList, Task> SendCharacterListOk;
        private Func<ulong, Task> SendCharacterListError;
        private Func<ulong, Task> SendCharacterPickOk;
        private Func<ulong, CharacterPickError, Task> SendCharacterPickError;
        private Func<ulong, Task> SendCharacterReleaseOk;
        private Func<ulong, Task> SendCharacterReleaseError;
        private Func<ulong, Task> SendCharacterCreateOk;
        private Func<ulong, CharacterCreateError, Task> SendCharacterCreateError;
                
        /// <summary>
        ///   A Post-Awake hook.
        /// </summary>
        protected override void Setup()
        {
            principalProtocol = GetComponent<PlayerProtocolServerSide>();
            authProtocol = GetComponent<EGAuthProtocolServerSide>();
            throttler = GetComponent<ServerSideThrottler>();
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
            SendCharacterListOk = MakeSender<CharactersNamesList>(EGGameProtocolDefinition.CharacterListOk);
            SendCharacterListError = MakeSender(EGGameProtocolDefinition.CharacterListError);
            SendCharacterPickOk = MakeSender(EGGameProtocolDefinition.CharacterPickOk);
            SendCharacterPickError = MakeSender<CharacterPickError>(EGGameProtocolDefinition.CharacterPickError);
            SendCharacterReleaseOk = MakeSender(EGGameProtocolDefinition.CharacterReleaseOk);
            SendCharacterReleaseError = MakeSender(EGGameProtocolDefinition.CharacterReleaseError);
            SendCharacterCreateOk = MakeSender(EGGameProtocolDefinition.CharacterCreateOk);
            SendCharacterCreateError = MakeSender<CharacterCreateError>(EGGameProtocolDefinition.CharacterCreateError);
        }

        private void AddAuthenticatedIncomingMessageHandler(
            string message, Func<ProtocolServerSide<EGGameProtocolDefinition>, ulong, Task> handler
        )
        {
            AddIncomingMessageHandler(message, authProtocol.LoginRequired(handler));
        }

        private void AddAuthenticatedIncomingMessageHandler<T>(
            string message, Func<ProtocolServerSide<EGGameProtocolDefinition>, ulong, T, Task> handler
        ) where T : ISerializable, new()
        {
            AddIncomingMessageHandler(message, authProtocol.LoginRequired(handler));
        }

        private void AddAuthThrottledCommandHandler(
            string message, Func<ulong, Task> handler, Func<ulong, DateTime, int, Task> onThrottled = null,
            int index = WalkThrottle
        )
        {
            onThrottled ??= (_, _, _) => Task.CompletedTask;
            AddAuthenticatedIncomingMessageHandler(message, async (proto, connId) =>
            {
                await throttler.DoThrottled(connId, async () =>
                {
                    try
                    {
                        await handler(connId);
                    }
                    catch (Exception e)
                    {
                        /* Handle this */
                        throw;
                    }
                }, onThrottled, index);
            });
        }

        private void AddAuthThrottledCommandHandler<T>(
            string message, Func<ulong, T, Task> handler, Func<ulong, DateTime, int, Task> onThrottled = null,
            int index = WalkThrottle
        ) where T : ISerializable, new()
        {
            onThrottled ??= (_, _, _) => Task.CompletedTask;
            AddAuthenticatedIncomingMessageHandler<T>(message, async (proto, connId, content) =>
            {
                await throttler.DoThrottled(connId, async () =>
                {
                    try
                    {
                        await handler(connId, content);
                    }
                    catch(Exception e) { /* Handle this */ }
                }, onThrottled, index);
            });
        }

        // Sends a list of their characters to the client.
        private async Task NotifyCharacterList(ulong connId)
        {
            Result<Character[], string> result = await authProtocol.ListCharacters(connId);
            if (result == null)
            {
                await SendCharacterListError(connId);
            }
            else
            {
                await SendCharacterListOk(
                    connId, new CharactersNamesList
                    {
                        CharacterNames = (from value in result.Element select value.DisplayName).ToArray()
                    }
                );
            }
        }

        /// <summary>
        ///   Initializes the protocol handlers once the server is ready.
        /// </summary>
        protected override void SetIncomingMessageHandlers()
        {
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.MoveDown, async (connId) => {
                principalProtocol.MoveDown(connId, true);
            });
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.MoveUp, async (connId) => {
                principalProtocol.MoveUp(connId, true);
            });
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.MoveLeft, async (connId) => {
                principalProtocol.MoveLeft(connId, true);
            });
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.MoveRight, async (connId) => {
                principalProtocol.MoveRight(connId, true);
            });
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.ClothRotate, async (connId) =>
            {
                CharacterServerSide character = principalProtocol.GetPrincipal(connId);
                character.RotateClothColor();
                authProtocol.GetCharacter(connId).ClothColor = character.ClothColor;
            }, null, ClothThrottle);
            AddAuthThrottledCommandHandler<String>(EGGameProtocolDefinition.Say, async (connId, content) =>
            {
                CharacterServerSide character = principalProtocol.GetPrincipal(connId);
                character.Say(content.Wrapped.Substring(0, EGGameProtocolDefinition.MaxSayLength));
            }, null, SayThrottle);
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.CharacterList, async (connId) =>
            {
                await NotifyCharacterList(connId);
            }, null, CharacterCommandThrottle);
            AddAuthThrottledCommandHandler<UInt>(EGGameProtocolDefinition.CharacterPick, async (connId, index) =>
            {
                try
                {
                    authProtocol.GetCharacter(connId);
                    await SendCharacterPickError(connId, new CharacterPickError {
                        Code = CharacterPickError.CharacterPickErrorCode.AlreadyPicked
                    });
                    return;
                }
                catch {}
                
                Result<Character[], string> result = await authProtocol.ListCharacters(connId);
                if (result == null)
                {
                    await SendCharacterPickError(connId, new CharacterPickError {
                        Code = CharacterPickError.CharacterPickErrorCode.UnknownError
                    });
                }
                else if (index < result.Element.Length)
                {
                    authProtocol.SetCharacter(connId, result.Element[index]);
                    await SendCharacterPickOk(connId);
                }
                else
                {
                    await SendCharacterPickError(connId, new CharacterPickError {
                        Code = CharacterPickError.CharacterPickErrorCode.InvalidIndex
                    });
                }
            });
            AddAuthThrottledCommandHandler(EGGameProtocolDefinition.CharacterRelease, async (connId) =>
            {
                try
                {
                    authProtocol.GetCharacter(connId);
                }
                catch (Exception e)
                {
                    await SendCharacterReleaseError(connId);
                }

                try
                {
                    await authProtocol.ClearCharacter(connId);
                }
                finally
                {
                    await SendCharacterReleaseOk(connId);
                }
            });
            AddAuthThrottledCommandHandler<CharacterCreationData>(EGGameProtocolDefinition.CharacterCreate,
                async (connId, data) =>
                {
                    try
                    {
                        authProtocol.GetCharacter(connId);
                        await SendCharacterCreateError(connId, new CharacterCreateError {
                            Code = CharacterCreateError.CharacterCreateErrorCode.AlreadyPicked
                        });
                        return;
                    }
                    catch {}
                    
                    try
                    {
                        Result<Character[], string> preResult = await authProtocol.ListCharacters(connId);
                        if (preResult.Code != ResultCode.Ok)
                        {
                            await SendCharacterCreateError(connId, new CharacterCreateError {
                                Code = CharacterCreateError.CharacterCreateErrorCode.UnknownError
                            });
                        }
                        else if (preResult.Element.Length >= MaxCharacters)
                        {
                            await SendCharacterCreateError(connId, new CharacterCreateError {
                                Code = CharacterCreateError.CharacterCreateErrorCode.MaxCharactersReached
                            });
                        }
                        
                        Result<Character, string> result = await authProtocol.CreateCharacter(connId, data);
                        switch (result.Code)
                        {
                            case ResultCode.Ok:
                            case ResultCode.Created:
                                await SendCharacterCreateOk(connId);
                                authProtocol.SetCharacter(connId, result.Element);
                                break;
                            case ResultCode.DuplicateKey:
                                await SendCharacterCreateError(connId, new CharacterCreateError {
                                    Code = CharacterCreateError.CharacterCreateErrorCode.DisplayNameInUse
                                });
                                break;
                            case ResultCode.ValidationError:
                                await SendCharacterCreateError(connId, new CharacterCreateError {
                                    Code = CharacterCreateError.CharacterCreateErrorCode.InvalidData,
                                    // Perhaps implement this? Errors = result.ValidationErrors
                                });
                                break;
                            default:
                                await SendCharacterCreateError(connId, new CharacterCreateError {
                                    Code = CharacterCreateError.CharacterCreateErrorCode.UnknownError
                                });
                                break;
                        }
                    }
                    catch
                    {
                        await SendCharacterCreateError(connId, new CharacterCreateError {
                            Code = CharacterCreateError.CharacterCreateErrorCode.UnknownError
                        });
                    }
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
            throttler.TrackConnection(clientId);
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
            throttler.UntrackConnection(clientId);
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
            throttler.Startup();
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
            throttler.Teardown();
        }
    }
}