namespace Kronos.Core.Serialization
{
    public interface ISerializable<T>
    {
        void Write(SerializationStream stream);
        void Read(DeserializationStream stream);
    }
}
