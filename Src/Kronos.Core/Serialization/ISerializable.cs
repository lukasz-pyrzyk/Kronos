namespace Kronos.Core.Serialization
{
    public interface ISerializable<T>
    {
        void Write(ref SerializationStream stream);
        void Read(ref DeserializationStream stream);
    }
}
