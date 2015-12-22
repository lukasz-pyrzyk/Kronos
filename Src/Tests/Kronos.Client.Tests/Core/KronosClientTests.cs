using System;
using System.IO;
using Kronos.Client.Core.Server;
using Kronos.Shared.Network.Models;
using NSubstitute;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Client.Tests.Core
{
    public class KronosClientTests
    {
        private readonly Fixture _fixture = new Fixture();


        [Fact]
        public void CanSendRequestByKeyStreamAndExpiryDate()
        {
            string key = _fixture.Create<string>();
            Stream stream = new MemoryStream(_fixture.Create<byte[]>());
            DateTime expiryDate = _fixture.Create<DateTime>();
            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;

            IKronosCommunicationService communicationService = Substitute.For<IKronosCommunicationService>();
            communicationService.SendToNode(Arg.Any<SocketRequest>()).Returns(expectedStatusCode);

            IKronosClient client = new KronosClient(communicationService);
            RequestStatusCode statusCode = client.SaveInCache(key, stream, expiryDate);

            Assert.Equal(statusCode, expectedStatusCode);
        }
    }
}
