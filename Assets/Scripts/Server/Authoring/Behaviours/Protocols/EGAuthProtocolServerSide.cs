using System;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using Protocols;
using Protocols.Messages;
using Server.Authoring.Behaviours.External;
using Server.Authoring.Types;

namespace Server.Authoring.Behaviours.Protocols
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    public class EGAuthProtocolServerSide : SimpleAuthProtocolServerSide<
        EGAuthProtocolDefinition, Nothing, LoginFailed, Kicked,
        string, AccountPreviewData, AccountData
    >
    {
        private MultiCharAccountAPIClient client = new MultiCharAccountAPIClient(
            "http://localhost:8080"
        );
        
        // Please note: The Nothing type is used when no data is needed.
        // This means that by default there is no need for any content
        // in the successful login response. See the EGAuthProtocolDefinition
        // type for more details.
    
        /// <summary>
        ///   Makes the handlers for the login messages.
        /// </summary>
        protected override void SetLoginMessageHandlers()
        {
            // For each defined login message in the protocol definition,
            // the handler must be created in this method. Since one login
            // message was defined in the protocol definition, one handler
            // is being created in this method.
            AddLoginMessageHandler<Login>("Default", async (login) => {
                // This method requires a totally custom implementation
                // from the user.
                //
                // Given the login details, they are either valid or
                // invalid. If the login is valid, then it must return:
                // - A successful response. By default, the successful
                //   response is defined of type Nothing, so the value
                //   will be Nothing.Instance.
                // - The account id, of type: string.
                // 
                // return AcceptLogin(successfulReason, accountId);
                //
                // Otherwise, for invalid login attempts, a rejection
                // reason must be generated, of type: LoginFailed.
                //
                // return RejectLogin(unsuccessfulReason);

                // WARNING: EVERY CALL TO AN EXTERNAL API OR USING A GAME OBJECT
                //          OR BEHAVIOUR MUST BE DONE IN THE CONTEXT OF A CALL TO
                //          RunInMainThread OR IT WILL SILENTLY FAIL.
                
                return RejectLogin(new LoginFailed().WithNotImplementedReason());
            });
        }
        
        /// <summary>
        ///   Retrieves the "full" account data, to store it in the session.
        /// </summary>
        protected override async Task<AccountData> FindAccount(string id)
        {
            // This doesn't mean that the returned data is actually the FULL
            // data of an account, but at least some sort of "long" representation
            // of the data that is sensible to retrieve when successfully doing
            // a login operation.
            //
            // This requires a mandatory implementation, for returning the default
            // value will cause the protocol to raise an exception and terminate
            // the session (& connection) abruptly and without even starting.

            // WARNING: EVERY CALL TO AN EXTERNAL API OR USING A GAME OBJECT
            //          OR BEHAVIOUR MUST BE DONE IN THE CONTEXT OF A CALL TO
            //          RunInMainThread OR IT WILL SILENTLY FAIL.

            return default(AccountData);
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