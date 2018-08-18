using System;
using System.Runtime.InteropServices;

namespace Kronos.Core.Serialization
{
    public static class NoAllocBitConverter
    {
        public static unsafe void GetBytes(short value, Span<byte> bytes)
        {
            MemoryMarshal.Write(bytes, ref value);
        }

        public static unsafe void GetBytes(int value, Span<byte> bytes)
        {
            MemoryMarshal.Write(bytes, ref value);
        }

        public static void GetBytes(long value, Span<byte> bytes)
        {
            MemoryMarshal.Write(bytes, ref value);
        }
    }
}