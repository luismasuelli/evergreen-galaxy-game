using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;

namespace Protocols {
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    public class AuthenticateProtocolDefinition : SimpleAuthProtocolDefinition<Nothing, Messages.LoginFailed, Messages.Kicked>
    {
        // Please note: The Nothing type is used when no data is needed.
        // This means that by default there is no need for any content
        // in the successful login response.
        //
        // Typically, that is the case. If you need a custom type, feel
        // free to use any of the concrete types in AlephVault.Unity.Binary
        // or create your own type implementing ISerializable interface,
        // pretty much as it occurs in the LoginFailed class.

        /// <summary>
        ///   Defines the login messages.
        /// </summary>
        protected override void DefineLoginMessages()
        {
            // Define many alternative login messages. Typically,
            // only one will be needed, however.
            DefineLoginMessage<Messages.Login>("Default");
        }
    }
}
