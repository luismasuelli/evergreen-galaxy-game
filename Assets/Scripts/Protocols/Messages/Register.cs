namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;

    public class Register : ISerializable
    {
        /**
         * A typical approach to a register class is to use
         * some sort of username and password authentication
         * mechanism. As always, ensure the Meetgard server
         * runs by enabling SSL, regardless on how you design
         * this class.
         *
         * Most of the scalar types are supported for this
         * serialization mechanism (which occurs by reference).
         * Also, custom types (also implementing ISerializable)
         * can be defined and nested here (typically as a
         * readonly value) and invoke their .Serialize method
         * as part of this class' .Serialize method.
         */
         
        public string Username;
        public string Password;
        public string PasswordConfirm;
        // Uncomment this one if your account uses a Display Name,
        // as part of the account instead of a separate data type.
        //
        // public string DisplayName;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Username);
            serializer.Serialize(ref Password);
            serializer.Serialize(ref PasswordConfirm);
            // Uncomment this one if your account uses a Display Name,
            // as part of the account instead of a separate data type.
            //
            // serializer.Serialize(ref DisplayName);
        }
    }
}
