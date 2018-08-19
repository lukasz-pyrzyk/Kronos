using System;
using System.Text;

namespace Kronos.Core.Serialization
{
    public static class UTF8Extensions
    {
        public static void GetBytes(this string content, Span<byte> bytes)
        {
            Encoding.UTF8.GetBytes(content, bytes);
        }

        public static Memory<byte> GetMemory(this string content)
        {
            return Encoding.UTF8.GetBytes(content);
        }

        public static string GetString(this Span<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public static string GetString(this ReadOnlySpan<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
