using AlephVault.Unity.Binary;

namespace Protocols.Messages
{
    /// <summary>
    ///   An error for when a character pick error occurs.
    /// </summary>
    public class CharacterCreateError : ISerializable
    {
        public enum CharacterCreateErrorCode
        {
            AlreadyPicked, DisplayNameInUse, InvalidData, MaxCharactersReached, UnknownError
        }

        public CharacterCreateErrorCode Code;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Code);
        }
    }
}