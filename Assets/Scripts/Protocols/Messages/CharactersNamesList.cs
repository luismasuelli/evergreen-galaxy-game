using AlephVault.Unity.Binary;

namespace Protocols.Messages
{
    public class CharactersNamesList : ISerializable
    {
        /// <summary>
        ///   The list of characters. The internal key order
        ///   is preserved in this list.
        /// </summary>
        public string[] CharacterNames;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref CharacterNames);
        }
    }
}
