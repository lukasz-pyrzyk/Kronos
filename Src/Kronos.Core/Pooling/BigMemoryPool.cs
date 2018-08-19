using System.Buffers;

namespace Kronos.Core.Pooling
{
    public class BigMemoryPool : MemoryPool<byte>
    {
        private readonly BigArrayPool _arrayPool = new BigArrayPool();

        public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
        {
            var array = _arrayPool.Rent(minBufferSize);
            return new RentedMemory(array, this);
        }

        public override int MaxBufferSize => BigArrayPool.MaxSize;

        public void Return(byte[] array)
        {
            _arrayPool.Return(array);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
