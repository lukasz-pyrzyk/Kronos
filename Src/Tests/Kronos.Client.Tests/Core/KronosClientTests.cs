using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Kronos.Client.Core;
using Kronos.Client.Core.Server;
using Kronos.Shared.Configuration;
using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;
using Kronos.Tests.Helpers;
using NSubstitute;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Client.Tests.Core
{
    public class KronosClientTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanInsertStreamToServer()
        {
            string key = _fixture.Create<string>();
            byte[] package =_fixture.Create<byte[]>();
            DateTime expiryDate = _fixture.Create<DateTime>();
            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;

            ICommunicationService communicationService = Substitute.For<ICommunicationService>();
            communicationService.SendToNode(Arg.Any<InsertRequest>(), Arg.Any<IPEndPoint>()).Returns(expectedStatusCode);

            IServerConfiguration configuration = Substitute.For<IServerConfiguration>();
            configuration.GetNodeForStream(Arg.Any<byte[]>()).Returns(new NodeConfiguration(_fixture.CreateIpAddress(), _fixture.Create<int>()));

            IKronosClient client = new KronosClient(communicationService, configuration);
            RequestStatusCode statusCode = client.InsertToServer(key, package, expiryDate);

            Assert.Equal(statusCode, expectedStatusCode);
        }
    }
}
