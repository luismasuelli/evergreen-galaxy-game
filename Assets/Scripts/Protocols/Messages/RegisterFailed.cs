namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;

    public class RegisterFailed : ISerializable
    {
        /**
         * A typical approach to this class is to define
         * the fields for a somewhat short message on why
         * did the register attempt failed.
         */
         
        public string Reason;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Reason);
        }
        
        public RegisterFailed WithPasswordMismatchError()
        {
            Reason = "Password and confirmation do not match";
            return this;
        }
        
        public RegisterFailed WithValidationError()
        {
            Reason = "Invalid username and/or password";
            return this;
        }
        
        public RegisterFailed WithUnknownError()
        {
            Reason = "An unknown error has occurred. Try again later";
            return this;
        }

        public RegisterFailed WithNotImplementedReason()
        {
            Reason = "Register is not yet implemented";
            return this;
        }
    }
}
