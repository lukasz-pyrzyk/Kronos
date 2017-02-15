using Kronos.Core.Pooling;

namespace Kronos.Core.Tests.Pooling
{
    public class PoolTests : BasePoolTests
    {
        protected override IPool<byte> Create(int count = 100)
        {
            return new Pool<byte>(count);
        }
    }
}
