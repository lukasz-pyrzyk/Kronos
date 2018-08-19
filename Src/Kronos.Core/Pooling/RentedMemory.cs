using System;
using System.Buffers;

namespace Kronos.Core.Pooling
{
    public readonly struct RentedMemory : IMemoryOwner<byte>
    {
        private readonly BigMemoryPool _pool;
        private readonly byte[] _array;

        public Memory<byte> Memory { get; }

        public RentedMemory(byte[] array, BigMemoryPool pool)
        {
            _pool = pool;
            _array = array;
            Memory = new Memory<byte>(array);
        }

        public void Dispose()
        {
            _pool.Return(_array);
        }
    }
}