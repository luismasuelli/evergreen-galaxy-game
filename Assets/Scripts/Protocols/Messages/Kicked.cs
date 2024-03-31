using System;
using AlephVault.Unity.Meetgard.Auth.Types;

namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;

    public class Kicked : IKickMessage<Kicked>
    {
        /**
         * A kicked message has some contexts that must
         * be defined, in order to give proper answers
         * when a kick was needed.
         */
         
        public string Reason;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Reason);
        }

        public Kicked WithAccountLoadErrorReason()
        {
            Reason = "An error has occurred while trying to load the account";
            return this;
        }

        public Kicked WithGhostedReason()
        {
            Reason = "The same account logged in from another client connection";
            return this;
        }

        public Kicked WithLoginTimeoutReason()
        {
            Reason = "Login timeout - the client took too much to login";
            return this;
        }

        public Kicked WithNonGracefulDisconnectionErrorReason(Exception reason)
        {
            Reason = $"Exception of type {reason?.GetType()?.FullName ?? "<graceful>"} on disconnection: {reason?.Message ?? "graceful"}";
            return this;
        }

        public Kicked WithSessionInitializationErrorReason()
        {
            Reason = $"An error has occurred while trying to initialize the session";
            return this;
        }
    }
}
