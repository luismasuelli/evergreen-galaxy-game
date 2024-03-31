using System;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using Protocols;
using Protocols.Messages;

namespace Client.Authoring.Behaviours.Protocols
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    public class AuthenticateProtocolClientSide : SimpleAuthProtocolClientSide<AuthenticateProtocolDefinition, Nothing, LoginFailed, Kicked>
    {
        // Please note: The Nothing type is used when no data is needed.
        // This means that by default there is no need for any content
        // in the successful login response. See the AuthenticateProtocolDefinition
        // type for more details.

        // A default login sender.
        public Func<Login, Task> DefaultLoginSender { get; private set; }

        /// <summary>
        ///   Makes the senders for the login messages.
        /// </summary>
        protected override void MakeLoginRequestSenders()
        {
            // For each defined login message in the protocol definition,
            // the sender must be created in this method. Since one login
            // message was defined in the protocol definition, one sender
            // is being created in this method.
            DefaultLoginSender = MakeLoginRequestSender<Login>("Default");
        }
                
        /**
         * This class has the following events that can be listened for. They are:
         *
         * - Handshake.OnWelcome = async () => { ... }; for when this client received
         *   from the server the first message. This client should, as immediately as
         *   possible, send the login message with some pre-fetched data.
         *
         * - Handshake.OnTimeout = async () => { ... }; for when this client received
         *   from the server a timeout message. This client should know the server will
         *   disconnect it immediately and also render a message, since the server did
         *   not receive, in certain threshold time, a login message.
         *
         * - OnLoginOK = async (ok) => { ... }; for when this client received a
         *   message telling the login was successful. The client should expect
         *   more messages from the server (e.g. account being set up and things
         *   needing a render in the client side).
         *
         * - OnLoginFailed = async (reason) => { ... }; for when this client
         *   received a message telling the login attempt was unsuccessful. This
         *   also implies that the client should consider this connection to be
         *   terminated automatically.
         *
         * - OnKicked = async (reason) => { ... }; for when this client received
         *   a message telling it was kicked. The reason is attached. This also
         *   implies that the client should consider this connection to be also
         *   terminated, and things should be cleared from the scene accordingly.
         *
         * - OnLoggedOut = async () => { ... }; for when this client received a
         *   logged out message. This means: the server processed and accepted
         *   a logout request from the client, cleared everything and terminated
         *   the connection. This implies that the client should consider the
         *   connection to be terminated, and things should be cleared from the
         *   scene accordingly.
         *
         * - OnAlreadyLoggedIn = async () => { ... }; for when the client received
         *   a message from the server telling that it is already logged in (with
         *   the same or another account). The client seems to be in some sort of
         *   locally inconsistent state to reach this message. It should fix its
         *   local state accordingly.
         *
         * - OnAccountAlreadyInUse = async () => { ... }; for when the client
         *   received a message from the server telling that the account is already
         *   in use. This looks pretty much as a login failure message, so clients
         *   receiving this message should consider that the connection terminated
         *   when receiving this message.
         *
         * - OnNotLoggedIn = async () => { ... }; for when the client received a
         *   message from the server telling that whatever message was sent by the
         *   client, it was not authorized since the client did not perform any
         *   kind of login yet.
         *
         * - OnForbidden = async () => { ... }; for when the client received a
         *   message from the server telling that whatever message was sent by
         *   the client, despite the client was logged in, it was not authorized
         *   to send that request.
         *
         * Additionally, there is a Logout() method to send a logout to the server.
         *
         * At any moment, it may be invoked as _ = Logout() or await Logout(). 
         */
    }
}