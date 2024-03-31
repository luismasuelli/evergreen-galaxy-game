namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;

    public class Login : ISerializable
    {
        /**
         * A typical approach to a login class is to use
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

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Username);
            serializer.Serialize(ref Password);
        }
    }
}
