using System;
using Kronos.Shared.Model;
using Kronos.Shared.Requests;
using Xunit;

namespace Kronos.Shared.Tests.Requests
{
    public class InsertRequestTests
    {
        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            CachedObject cachedObject = new CachedObject("key", new byte[5], DateTime.MaxValue);

            InsertRequest request = new InsertRequest(cachedObject);
            
            Assert.NotNull(request);
        }
    }
}
