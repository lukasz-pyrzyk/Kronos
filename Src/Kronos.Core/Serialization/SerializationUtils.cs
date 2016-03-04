using System.IO;
using ProtoBuf;

namespace Kronos.Core.Serialization
{
    public static class SerializationUtils
    {
        private const PrefixStyle PrefixStyle = ProtoBuf.PrefixStyle.Fixed32;

        public static int GetLengthOfPackage(byte[] buffer)
        {
            int size;
            Serializer.TryReadLengthPrefix(buffer, 0, buffer.Length, PrefixStyle, out size);
            return size;
        }

        public static byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(ms, obj, PrefixStyle);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle);
            }
        }
    }
}
