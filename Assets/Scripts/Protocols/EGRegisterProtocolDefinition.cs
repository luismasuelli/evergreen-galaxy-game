using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;

namespace Protocols {
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    public class EGRegisterProtocolDefinition : SimpleRegisterProtocolDefinition<Nothing, Messages.RegisterFailed>
    {
        // Please note: The Nothing type is used when no data is needed.
        // This means that by default there is no need for any content
        // in the successful register response.
        //
        // Typically, that is the case. If you need a custom type, feel
        // free to use any of the concrete types in AlephVault.Unity.Binary
        // or create your own type implementing ISerializable interface,
        // pretty much as it occurs in the RegisterFailed class.

        /// <summary>
        ///   Defines the register messages.
        /// </summary>
        protected override void DefineRegisterMessages()
        {
            // Define many alternative register messages. Typically,
            // only one will be needed, however.
            DefineRegisterMessage<Messages.Register>("Default");
        }
    }
}
