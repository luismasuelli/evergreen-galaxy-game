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

    public class RegisterAccountProtocolClientSide : SimpleRegisterProtocolClientSide<RegisterAccountProtocolDefinition, Nothing, RegisterFailed>
    {
        // Please note: The Nothing type is used when no data is needed.
        // This means that by default there is no need for any content
        // in the successful register response. See the RegisterAccountProtocolDefinition
        // type for more details.

        // A default register sender.
        public Func<Register, Task> DefaultRegisterSender { get; private set; }

        /// <summary>
        ///   Makes the senders for the register messages.
        /// </summary>
        protected override void MakeRegisterRequestSenders()
        {
            // For each defined register message in the protocol definition,
            // the sender must be created in this method. Since one register
            // message was defined in the protocol definition, one sender
            // is being created in this method.
            DefaultRegisterSender = MakeRegisterRequestSender<Register>("Default");
        }
        
        /**
         * This class has the following events that can be listened for. They are:
         *
         * - Handshake.OnWelcome = async () => { ... }; for when this client received
         *   from the server the first message. This client should, as immediately as
         *   possible, send the register message with some pre-fetched data.
         *
         * - Handshake.OnTimeout = async () => { ... }; for when this client received
         *   from the server a timeout message. This client should know the server will
         *   disconnect it immediately and also render a message, since the server did
         *   not receive, in certain threshold time, a register message.
         *
         * - OnRegisterOK = async (ok) => { ... }; for when this client received a
         *   message telling the register was successful. The client should expect
         *   more messages from the server (e.g. account being set up and things
         *   needing a render in the client side).
         *
         * - OnRegisterFailed = async (reason) => { ... }; for when this client
         *   received a message telling the register attempt was unsuccessful.
         *   This also implies that the client should consider this connection
         *   to be terminated automatically.
         */
    }
}
