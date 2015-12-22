using System;
using System.IO;
using Kronos.Shared.Network.Requests;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Shared.Tests.Network.Model
{
    public class SocketRequestTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            string key = _fixture.Create<string>();
            Stream stream = new MemoryStream(_fixture.Create<byte[]>());
            DateTime expiryDate = _fixture.Create<DateTime>();

            InsertRequest request = new InsertRequest(key, stream, expiryDate);

            Assert.Equal(key, request.Key);
            Assert.Equal(stream, request.Stream);
            Assert.Equal(expiryDate, request.ExpiryDate);
            Assert.Equal(stream.Length, request.Stream.Length);
        }
    }
}
