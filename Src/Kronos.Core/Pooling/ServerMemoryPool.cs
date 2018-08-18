using System.Buffers;

namespace Kronos.Core.Pooling
{
    public class ServerMemoryPool
    {
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

        public IMemoryOwner<byte> Rent(int i)
        {
            var owner = _pool.Rent(i);
            return owner;
        }
    }
}
