namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;

    public class AccountPreviewData : ISerializable
    {
        /**
         * The account preview stands for a small part
         * of the account data to send as preliminary.
         *
         * By default, this implementation covers only
         * the username, but can be changed according
         * to the game needs.
         */
         
        public string Username;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Username);
        }
    }
}
