using System.Buffers;

namespace Kronos.Core.Pooling
{
    public class ServerMemoryPool
    {
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

        public IMemoryOwner<byte> Rent(int count)
        {
            var owner = _pool.Rent(count);
            return owner;
        }
    }
}
