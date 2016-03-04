using System.IO;
using ProtoBuf;

namespace Kronos.Core.Serialization
{
    public class SerializationUtils
    {
        public const PrefixStyle Style = PrefixStyle.Fixed32;

        public static int GetLengthOfPackage(byte[] buffer)
        {
            int size;
            Serializer.TryReadLengthPrefix(buffer, 0, buffer.Length, Style, out size);
            return size;
        }

        public static byte[] Serialize<T>(T obj)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(ms, obj, Style);
                buffer = ms.ToArray();
            }
            return buffer;
        }

        public byte[] SerializeZ(object obj)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(ms, obj, Style);
                buffer = ms.ToArray();
            }
            return buffer;
        }


        public static T Deserialize<T>(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Fixed32);
            }
        }
    }
}
