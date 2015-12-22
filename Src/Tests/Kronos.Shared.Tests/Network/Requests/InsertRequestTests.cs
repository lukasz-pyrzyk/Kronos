using System;
using System.IO;
using System.Net;
using Kronos.Shared.Network.Requests;
using Kronos.Tests.Helpers;
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
            Stream stream = new MemoryStream(_fixture.Create<byte[]>());
            DateTime expiryDate = _fixture.Create<DateTime>();
            string host = _fixture.CreateIpAddress();
            int port = _fixture.Create<int>();

            InsertRequest request = new InsertRequest(key, stream, expiryDate, host, port);

            Assert.Equal(key, request.Key);
            Assert.Equal(stream, request.Stream);
            Assert.Equal(expiryDate, request.ExpiryDate);
            Assert.Equal(stream.Length, request.Stream.Length);
        }
    }
}
