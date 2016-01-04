using System;
using Kronos.Shared.Network.Model;
using Kronos.Shared.Network.Requests;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Shared.Tests.Network.Requests
{
    public class InsertRequestTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            CachedObject cachedObject = _fixture.Create<CachedObject>();

            InsertRequest request = new InsertRequest(cachedObject);
            
            Assert.NotNull(request);
        }
    }
}
