namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;

    public class LoginFailed : ISerializable
    {
        /**
         * A typical approach to this class is to define
         * the fields for a somewhat short message on why
         * did the login attempt failed.
         */
         
        public string Reason;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Reason);
        }
        
        public LoginFailed WithNotImplementedReason()
        {
            Reason = "Login is not yet implemented";
            return this;
        }
    }
}
