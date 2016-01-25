using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kronos.Core.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    public abstract class Request
    {
        public abstract byte[] Serialize();

        protected static byte[] JoinWithTotalSize(params byte[][] arrays)
        {
            int packagesSize = arrays.Sum(x => x.Length);
            byte[] packageSizeBytes = Serialize(packagesSize);

            int intSize = sizeof(int);
            byte[] finalArray = new byte[intSize + packagesSize];

            Buffer.BlockCopy(packageSizeBytes, 0, finalArray, 0, packageSizeBytes.Length);

            int offset = intSize;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, finalArray, offset, data.Length);
                offset += data.Length;
            }

            return finalArray;
        }

        protected static byte[] Serialize(int value)
        {
            return BitConverter.GetBytes(value);
        }

        protected static byte[] Serialize(long value)
        {
            return BitConverter.GetBytes(value);
        }

        protected static byte[] Serialize(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        protected static byte[] Serialize(DateTime value)
        {
            return BitConverter.GetBytes(value.Ticks);
        }

        protected static int DeserializeInt(byte[] stream)
        {
            return BitConverter.ToInt32(stream, 0);
        }

        protected static DateTime DeseriazeDatetime(byte[] stream)
        {
            return DateTime.FromBinary(DeserializeLong(stream));
        }

        protected static long DeserializeLong(byte[] stream)
        {
            return BitConverter.ToInt64(stream, 0);
        }

        public static string DeserializeString(byte[] stream)
        {
            return Encoding.UTF8.GetString(stream);
        }

        protected static byte[] GetPartOfStream<T>(byte[] stream, ref int offset)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            int partSize = Marshal.SizeOf(typeof(T));
            return GetPartOfStream(stream, ref offset, partSize);
        }

        protected static byte[] GetPartOfStream(byte[] stream, ref int offset, int partSize)
        {
            byte[] keyPackageSizeBytes = new byte[partSize];
            Array.ConstrainedCopy(stream, offset, keyPackageSizeBytes, 0, keyPackageSizeBytes.Length);
            offset += partSize;
            return keyPackageSizeBytes;
        }
    }
}
