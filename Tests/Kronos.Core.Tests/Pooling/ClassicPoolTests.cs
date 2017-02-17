using Kronos.Core.Pooling;

namespace Kronos.Core.Tests.Pooling
{
    public class ClassicPoolTests : PoolTests
    {
        protected override Pool<byte> Create(int count = 100)
        {
            return new ClassicPool<byte>(count);
        }
    }
}
