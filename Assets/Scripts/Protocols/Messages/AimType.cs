namespace Protocols.Messages
{
    using AlephVault.Unity.Binary;

    public class AimType : ISerializable
    {
        public int Map;
        public ushort X;
        public ushort Y;

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref Map);
            serializer.Serialize(ref X);
            serializer.Serialize(ref Y);
        }
    }
}
