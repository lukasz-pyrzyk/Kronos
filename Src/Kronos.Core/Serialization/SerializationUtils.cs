using System.IO;
using ProtoBuf;

namespace Kronos.Core.Serialization
{
    public static class SerializationUtils
    {
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] stream)
        {
            using (MemoryStream ms = new MemoryStream(stream))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
