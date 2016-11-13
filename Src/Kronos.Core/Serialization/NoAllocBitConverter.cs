namespace Kronos.Core.Serialization
{
    /// <summary>
    /// Provides methods not allocating the byte buffer.
    /// Author: Szymon Kulec, see https://github.com/akkadotnet/Wire/blob/dev/Wire/NoAllocBitConverter.cs
    /// </summary>
    public static class NoAllocBitConverter
    {
        public static void GetBytes(char value, byte[] bytes)
        {
            GetBytes((short)value, bytes);
        }

        public static unsafe void GetBytes(short value, byte[] bytes)
        {
            fixed (byte* b = bytes)
                *(short*)b = value;
        }

        public static unsafe void GetBytes(int value, byte[] bytes)
        {
            fixed (byte* b = bytes)
                *(int*)b = value;
        }

        public static unsafe void GetBytes(long value, byte[] bytes)
        {
            fixed (byte* b = bytes)
                *(long*)b = value;
        }

        public static void GetBytes(ushort value, byte[] bytes)
        {
            GetBytes((short)value, bytes);
        }

        public static void GetBytes(uint value, byte[] bytes)
        {
            GetBytes((int)value, bytes);
        }

        public static void GetBytes(ulong value, byte[] bytes)
        {
            GetBytes((long)value, bytes);
        }

        public static unsafe void GetBytes(float value, byte[] bytes)
        {
            GetBytes(*(int*)&value, bytes);
        }

        public static unsafe void GetBytes(double value, byte[] bytes)
        {
            GetBytes(*(long*)&value, bytes);
        }
    }
}