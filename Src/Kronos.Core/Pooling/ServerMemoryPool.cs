using System.Buffers;

namespace Kronos.Core.Pooling
{
    public class ServerMemoryPool
    {
        private readonly BigMemoryPool _pool = new BigMemoryPool();

        public IMemoryOwner<byte> Rent(int count)
        {
            return _pool.Rent(count);
        }
    }
}
