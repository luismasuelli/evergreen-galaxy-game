using Core.Types.Characters;

namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;

    public class CharacterCreationData : ISerializable
    {
        public string DisplayName;
        public SexType Sex;
        public HairType Hair;
        public HairColorType HairColor;
        public RaceType Race;
    
        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref DisplayName);
            serializer.Serialize(ref Sex);
            serializer.Serialize(ref Hair);
            serializer.Serialize(ref HairColor);
            serializer.Serialize(ref Race);
        }
    }
}
