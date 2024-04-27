using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Core.Types.Characters;
using Newtonsoft.Json.Linq;
using Protocols;
using Protocols.Messages;
using Server.Authoring.Behaviours.External;
using Server.Authoring.Behaviours.External.Models;
using Server.Authoring.Behaviours.NetworkObjects;
using Server.Authoring.Types;
using UnityEngine;

namespace Server.Authoring.Behaviours.Protocols
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    [RequireComponent(typeof(PlayerProtocolServerSide))]
    public class EGAuthProtocolServerSide : SimpleAuthProtocolServerSide<
        EGAuthProtocolDefinition, Nothing, LoginFailed, Kicked,
        string, AccountPreviewData, AccountData
    >
    {
        private const float WorldSaveInterval = 600f;
        private float worldSavePhase;
        private HashSet<ulong> activeSessions = new();
        
        private MultiCharAccountAPIClient client = new MultiCharAccountAPIClient(
            "http://localhost:8080"
        );

        private PlayerProtocolServerSide playerProtocol;
        
        // Please note: The Nothing type is used when no data is needed.
        // This means that by default there is no need for any content
        // in the successful login response. See the EGAuthProtocolDefinition
        // type for more details.

        protected override void Setup()
        {
            base.Setup();
            playerProtocol = GetComponent<PlayerProtocolServerSide>();
            OnSessionStarting += HandleSessionStarted;
            OnSessionTerminating += HandleSessionTerminating;
        }
    
        /// <summary>
        ///   Makes the handlers for the login messages.
        /// </summary>
        protected override void SetLoginMessageHandlers()
        {
            // For each defined login message in the protocol definition,
            // the handler must be created in this method. Since one login
            // message was defined in the protocol definition, one handler
            // is being created in this method.
            AddLoginMessageHandler<Login>("Default", async login => {
                Result<MultiCharAccount, string> result = await RunInMainThread(() => client.FindAccountByLogin(login.Username.ToLower().Trim()));
                if (result.Code == ResultCode.Ok)
                {
                    // This is not a proper way to compare a user's
                    // password. Passwords should be encrypted in
                    // production environments, and the storage is
                    // not the appropriate place to do this.
                    //
                    // Otherwise, modify the by-login method in the
                    // storage instead of retrieving the account and
                    // performing the logic here.
                    if (login.Password == result.Element.Password)
                    {
                        return AcceptLogin(Nothing.Instance, result.Element.Id);
                    }
                    return RejectLogin(new LoginFailed { Reason = "mismatch" });
                }
                if (result.Code == ResultCode.DoesNotExist)
                {
                    // Here, fake a check of an encrypted password
                    // when moving this to a production environment.
                    return RejectLogin(new LoginFailed { Reason = "mismatch" });
                }
                if (result.Code == ResultCode.BadRequest)
                {
                    // The name was not provided
                    return RejectLogin(new LoginFailed() { Reason = "missing_username" });
                }
                Debug.Log($"Result: {result.Code}");
                return RejectLogin(new LoginFailed() { Reason = "server_error" });
            });
        }
        
        /// <summary>
        ///   Retrieves the "full" account data, to store it in the session.
        /// </summary>
        protected override async Task<AccountData> FindAccount(string id)
        {
            Result<MultiCharAccount, string> result = await RunInMainThread(
                () => client.GetAccount(id)
            );
            if (result.Code == ResultCode.Ok)
            {
                return new AccountData { Account = result.Element };
            }
            if (result.Code == ResultCode.DoesNotExist)
            {
                return null;
            }
            throw new Exception("auth_lookup_error");
        }
        
        /// <summary>
        ///   Retrieves what happens when an account tries to login (from another
        ///   connection) considering that it is already logged in in one connection.
        /// </summary>
        protected override AccountAlreadyLoggedManagementMode IfAccountAlreadyLoggedIn()
        {
            // The values are:
            // - AccountAlreadyLoggedManagementMode.Ghost, to kick the previous connection.
            // - AccountAlreadyLoggedManagementMode.Reject, to reject the new connection.
            // - AccountAlreadyLoggedManagementMode.AllowAll, to allow all the connections.
            return AccountAlreadyLoggedManagementMode.Ghost;
        }
        
        /// <summary>
        ///   Handles an error that occurred in the session management.
        /// </summary>
        protected override async Task OnSessionError(ulong clientId, SessionStage stage, System.Exception error)
        {
            // The stage value can be:
            // - SessionStage.AccountLoad: The account data was being loaded.
            // - SessionStage.Initialization: The session was being initialized.
            // - SessionStage.PermissionCheck: The session was checking a permission for a LoginRequired message.
            // - SessionStage.Termination: The session was terminating.

            // WARNING: EVERY CALL TO AN EXTERNAL API OR USING A GAME OBJECT
            //          OR BEHAVIOUR MUST BE DONE IN THE CONTEXT OF A CALL TO
            //          RunInMainThread OR IT WILL SILENTLY FAIL.
            Debug.Log($">>> OnSessionError {clientId} {stage}");
            Debug.LogException(error);
        }

        /// <summary>
        ///   Retrieves a list of characters for an account.
        /// </summary>
        /// <param name="connId">The connection id</param>
        /// <returns>The result of the request</returns>
        public async Task<Result<Character[], string>> ListCharacters(ulong connId)
        {
            if (!TryGetSessionData(connId, "account", out object data))
            {
                return null;
            }
            else
            {
                string id = ((AccountData)data).GetID();
                return await RunInMainThread(() => client.ListCharacters(id, true));
            }
        }

        /// <summary>
        ///   Sets the currently selected character into the session.
        /// </summary>
        /// <param name="clientId">The id of the client to set the character to</param>
        /// <param name="character">The character</param>
        public void SetCharacter(ulong clientId, Character character)
        {
            SetSessionData(clientId, "character", character);
            playerProtocol.InstantiateCharacter(clientId, character);
        }

        /// <summary>
        ///   Gets the currently selected character from the session.
        /// </summary>
        /// <param name="clientId">The id of the client to get the character from</param>
        /// <returns></returns>
        public Character GetCharacter(ulong clientId)
        {
            return (Character)GetSessionData(clientId, "character");
        }

        /// <summary>
        ///   Clears the currently selected character from the session.
        /// </summary>
        /// <param name="clientId">The id of the client to clear the character from</param>
        /// <returns>The result of the post-mortem Save request</returns>
        public async Task<Result<Character, string>> ClearCharacter(ulong clientId)
        {
            Character character;
            try
            {
                character = GetCharacter(clientId);
            }
            catch
            {
                return null;
            }

            // It will be removed and saved.
            playerProtocol.RemoveCharacter(clientId);
            RemoveSessionData(clientId, "character");

            return await RunInMainThread(() => client.UpdateCharacter(character.Id, new JObject
            {
                { "$set", JObject.FromObject(character) }
            }));
        }

        public async Task<Result<Character, string>> CreateCharacter(ulong clientId, CharacterCreationData data)
        {
            Character character = new Character
            {
                AccountId = ((AccountData)GetSessionData(clientId, "account")).GetID(),
                DisplayName = data.DisplayName,
                Sex = data.Sex, Race = data.Race,
                Hair = data.Hair, HairColor = data.HairColor,
                ClothColor = ClothColorType.None,
                Position = new Position
                {
                    Scope = PlayerProtocolServerSide.DefaultScope,
                    Map = PlayerProtocolServerSide.DefaultMap,
                    X = 50, Y = 50
                }
            };
            Result<Character, string> result = await RunInMainThread(() => client.CreateCharacter(character));
            if (result.Code != ResultCode.Ok)
            {
                return result;
            }

            character.Id = result.CreatedID;
            return new Result<Character, string>
            {
                Code = ResultCode.Ok, Element = character, CreatedID = character.Id
            };
        }

        private async Task HandleSessionStarted(ulong clientId, AccountData data)
        {
            SetSessionData(clientId, "account", data);
            activeSessions.Add(clientId);
        }

        private async Task HandleSessionTerminating(ulong clientId, Kicked reason)
        {
            activeSessions.Remove(clientId);
            AccountData account = (AccountData)GetSessionData(clientId, "account");
            Result<MultiCharAccount, string> result = await RunInMainThread(
                () => client.UpdateAccount(account.GetID(), new JObject
                {
                    { "$set", JObject.FromObject(account.Account) }
                })
            );
            if (result.Code != ResultCode.Ok)
            {
                Debug.LogWarning(
                    $"WARNING: Session for user {account.Account.Login} ({clientId}) " +
                    $"could not be stored because of an error code: {result.Code}"
                );
                return;
            }

            Result<Character, string> result2 = await ClearCharacter(clientId);
            if (result2.Code != ResultCode.Ok)
            {
                Debug.LogWarning(
                    $"WARNING: Session for user {account.Account.Login} ({clientId}) " +
                    $"could not store the character out of an error code: {result2.Code}"
                );
            }
        }

        protected void Update()
        {
            if (!server.IsRunning) return;

            worldSavePhase += Time.deltaTime;
            if (worldSavePhase > WorldSaveInterval)
            {
                worldSavePhase -= WorldSaveInterval;

                foreach (ulong clientId in activeSessions)
                {
                    try
                    {
                        AccountData account = (AccountData)GetSessionData(clientId, "account");
                        RunInMainThread(() => client.UpdateAccount(account.GetID(), new JObject
                        {
                            { "$set", JObject.FromObject(account) }
                        }));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    
                    try
                    {
                        CharacterServerSide character = playerProtocol.GetPrincipal(clientId);
                        if (!character.NetRoseScopeServerSide) continue;
                        character.Save();
                        Character characterModel = GetCharacter(clientId);
                        RunInMainThread(() => client.UpdateCharacter(characterModel.Id, new JObject
                        {
                            { "$set", JObject.FromObject(characterModel) }
                        }));
                    }
                    catch (Exception) {}
                }
            }
        }

        /**
         * This class has the following events that can be listened for. They are:
         *
         * - OnSessionStarting = async (connection, fullAccountData) => { ... }; for when
         *   the login was successful and the session is starting. The session should
         *   store some data by grabbing it from the fullAccountData.
         *
         * - OnSessionTerminating = async (connection, reason) => { ... }; for when the account
         *   is being disconnected and the session is terminating for it. However, for
         *   that moment the session data still exists, and this is a last opportunity
         *   to read data from it and do something with it.
         *
         * This class also defines some public methods that can be used from this or
         * other protocols. They're:
         *
         * - await Kick(accountId, reason) to force disconnecting an account.
         * - SessionExists(accountId) to know whether an account is logged in.
         * - (SomeType)GetSessionData(connection, key) to get some data from the session.
         * - TryGetSessionData(connection, key, out val) ? (SomeType)val : default(SomeType)
         *   to get some data from the session without risking a KeyNotFoundException.
         * - SetSessionData(connection, key, val) to set data in the session.
         * - SessionContainsKey(connection, key) to know whether that key is set in the session.
         * - RemoveSessionData(connection, key) to clear a key from the session.
         * - ClearSessionUserData(connection) to clear the custom session data.
         *   ClearSessionUserData(connection, true) to include the internal session's core keys.
         *
         * Finally, this class has methods that are of interest of a login protocol: those for
         * actually restricting requests from non-logged users. These methods are:
         *
         * - handler = LoginRequired([async (connection) => { ... return whether connection is allowed}, ]handler)
         *   Checks whether the connection is logged in and allowed to perform the action.
         *   A new handler is returned, which typically becomes the value for a call to
         *   AddIncomingMessageHandler in another protocol.
         *   - This method has TWO versions: one for untyped handlers, and one for typed ones.
         *   - The first parameter is optional. If not set, then no allowance check is done.
         * - handler = LogoutRequired(handler)
         *   Checks whether the connection is NOT logged in.
         *   A new handler is returned, which typically becomes the value for a call to
         *   AddIncomingMessageHandler in another protocol.
         *   - This method has TWO versions: one for untyped handlers, and one for typed ones.
         */
    }
}