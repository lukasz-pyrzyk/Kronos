using Kronos.Core.Pooling;

namespace Kronos.Core.Tests.Pooling
{
    public class ConcurrentPoolTests : BasePoolTests
    {
        protected override IPool<byte> Create(int count = 100)
        {
            return new ConcurrentPool<byte>(count);
        }
    }
}
