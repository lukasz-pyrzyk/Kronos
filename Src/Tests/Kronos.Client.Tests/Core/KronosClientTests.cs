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
            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;

            var communicationServiceMock = new Mock<ICommunicationService>();
            communicationServiceMock.Setup(x => x.SendToNode(It.IsAny<InsertRequest>(), It.IsAny<IPEndPoint>())).Returns(expectedStatusCode);
            
            IKronosClient client = new KronosClient(communicationServiceMock.Object, It.IsAny<IPEndPoint>());
            RequestStatusCode statusCode = client.InsertToServer(key, package, expiryDate);

            Assert.Equal(statusCode, expectedStatusCode);
        }

        [Fact]
        public void CanInsertCachedObjectToServer()
        {
            CachedObject objectToCache = new CachedObject("key", Encoding.UTF8.GetBytes("package"), DateTime.Today.AddDays(1));
            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;

            var communicationServiceMock = new Mock<ICommunicationService>();
            communicationServiceMock.Setup(x => x.SendToNode(It.IsAny<InsertRequest>(), It.IsAny<IPEndPoint>())).Returns(expectedStatusCode);
            
            IKronosClient client = new KronosClient(communicationServiceMock.Object, It.IsAny<IPEndPoint>());
            RequestStatusCode statusCode = client.InsertToServer(objectToCache);

            Assert.Equal(statusCode, expectedStatusCode);
        }
    }
}
