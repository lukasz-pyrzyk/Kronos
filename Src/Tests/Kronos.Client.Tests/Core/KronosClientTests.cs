using System;
using System.Net;
using System.Text;
using Kronos.Client.Transfer;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;
using Moq;
using Xunit;

namespace Kronos.Client.Tests.Core
{
    public class KronosClientTests
    {
        [Fact]
        public void CanInsertObjectByKeyPackageAndExpiryDateToServer()
        {
            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("package");
            DateTime expiryDate = DateTime.Today.AddDays(1);

            var communicationServiceMock = new Mock<ICommunicationService>();

            IKronosClient client = new KronosClient(communicationServiceMock.Object, It.IsAny<IPEndPoint>());
            client.InsertToServer(key, package, expiryDate);

            communicationServiceMock.Verify(x => x.SendToNode(It.IsAny<InsertRequest>(), It.IsAny<IPEndPoint>()), Times.Exactly(1));
        }
        [Fact]
        public void CanReadValueFromCache()
        {
            var communicationServiceMock = new Mock<ICommunicationService>();

            IKronosClient client = new KronosClient(communicationServiceMock.Object, It.IsAny<IPEndPoint>());
            client.TryGetValue(It.IsAny<string>());

            communicationServiceMock.Verify(x => x.SendToNode(It.IsAny<GetRequest>(), It.IsAny<IPEndPoint>()), Times.Exactly(1));
        }
    }
}
