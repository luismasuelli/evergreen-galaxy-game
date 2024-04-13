using AlephVault.Unity.Binary;

namespace Models
{
    /// <summary>
    ///   This is the refresh message for the characters.
    ///   It only adds serialization.
    /// </summary>
    public class CharacterRefreshData : CharacterCommonData
    {
        public override void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref ClothColor);
        }
    }
}
