using System;
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
                SerializeToStream(ms, obj);
                buffer = ms.ToArray();
            }
            return buffer;
        }

        public static byte[] SerializeToStreamWithLength<T>(T obj)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                SerializeToStreamWithLength(ms, obj, Style);
                buffer = ms.ToArray();
            }
            return buffer;
        }

        public static void SerializeToStream<T>(Stream stream, T obj)
        {
            Serializer.Serialize(stream, obj);
        }

        public static void SerializeToStreamWithLength<T>(Stream stream, T obj, PrefixStyle style)
        {
            Serializer.SerializeWithLengthPrefix(stream, obj, style);
        }

        public static T Deserialize<T>(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static T DeserializeWithLength<T>(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Serializer.DeserializeWithLengthPrefix<T>(ms, Style);
            }
        }
    }
}

