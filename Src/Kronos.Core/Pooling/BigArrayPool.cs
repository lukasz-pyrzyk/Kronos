using System.Buffers;

namespace Kronos.Core.Pooling
{
    public class BigArrayPool
    {
        public const int MaxSize = 5 * 1024 * 1024;

        private static readonly object Locker = new object();
        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create(MaxSize, 10);

        public byte[] Rent(int minimumLength)
        {
            lock (Locker)
            {
                return _pool.Rent(minimumLength);
            }
        }

        public void Return(byte[] array)
        {
            lock (Locker)
            {
                _pool.Return(array);
            }
        }
    }
}
