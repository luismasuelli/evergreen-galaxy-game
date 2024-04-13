using AlephVault.Unity.Binary;

namespace Models
{
    /// <summary>
    ///   This is the refresh message for the characters.
    ///   It only adds serialization.
    /// </summary>
    public class CharacterRefreshData : CharacterCommonData
    {
        /// <summary>
        ///   The text being said.
        /// </summary>
        public string Text;
        
        public override void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref ClothColorValue);
            serializer.Serialize(ref Text);
        }
    }
}
