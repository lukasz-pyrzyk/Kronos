using System.IO;
using ProtoBuf;

namespace Kronos.Core.Serialization
{
    public class SerializationUtils
    {
        public const PrefixStyle Style = PrefixStyle.Fixed32;

        public static byte[] Serialize<T>(T obj)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                SerializeToStream(ms, obj);
                buffer = ms.ToArray();
            }
            return buffer;
        }

        public static void SerializeToStream<T>(Stream stream, T obj)
        {
            Serializer.Serialize(stream, obj);
        }

        public static void SerializeToStreamWithLength<T>(Stream stream, T obj)
        {
            Serializer.SerializeWithLengthPrefix(stream, obj, Style);
        }

        public static T Deserialize<T>(byte[] buffer, int? count = null)
        {
            using (MemoryStream ms = new MemoryStream(buffer, 0, count ?? buffer.Length))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}

