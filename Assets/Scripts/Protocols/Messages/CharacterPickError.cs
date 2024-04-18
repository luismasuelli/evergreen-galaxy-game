using AlephVault.Unity.Binary;

namespace Protocols.Messages
{
    /// <summary>
    ///   An error for when a character pick error occurs.
    /// </summary>
    public class CharacterPickError : ISerializable
    {
        public enum CharacterPickErrorCode
        {
            InvalidIndex, AlreadyPicked, UnknownError
        }

        public CharacterPickErrorCode Code;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Code);
        }
    }
}