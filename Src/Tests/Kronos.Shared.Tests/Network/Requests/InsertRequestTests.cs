using System;
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
            string key = _fixture.Create<string>();
            byte[] package = _fixture.Create<byte[]>();
            DateTime expiryDate = _fixture.Create<DateTime>();

            InsertRequest request = new InsertRequest(key, package, expiryDate);

            Assert.Equal(key, request.Key);
            Assert.Equal(package, request.Package);
            Assert.Equal(expiryDate, request.ExpiryDate);
            Assert.Equal(package.Length, request.Package.Length);
        }
    }
}
