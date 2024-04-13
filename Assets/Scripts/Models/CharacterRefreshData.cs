using AlephVault.Unity.Binary;

namespace Models
{
    public class CharacterRefreshData : CharacterCommonData
    {
        public override void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref ClothColor);
        }
    }
}
