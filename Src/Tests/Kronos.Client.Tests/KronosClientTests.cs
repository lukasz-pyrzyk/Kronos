using System;
using System.Text;
using Kronos.Client.Transfer;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Moq;
using Xunit;

namespace Kronos.Client.Tests
{
    public class KronosClientTests
    {
        [Fact]
        public void CanInsertObject()
        {
            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("package");
            DateTime expiryDate = DateTime.Today.AddDays(1);
            
            var communicationServiceMock = new Mock<IClientServerConnection>();

            IKronosClient client = new KronosClient(communicationServiceMock.Object);
            client.InsertToServer(key, package, expiryDate);

            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<InsertRequest>()), Times.Exactly(1));
        }

        [Fact]
        public void CanGetObject()
        {
            const string word = "lorem ipsum";

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(It.IsAny<Request>()))
                .Returns(SerializationUtils.Serialize(word));

            IKronosClient client = new KronosClient(communicationServiceMock.Object);
            byte[] response = client.TryGetValue(It.IsAny<string>());
            string responseString = SerializationUtils.Deserialize<string>(response);

            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<GetRequest>()), Times.Exactly(1));

            Assert.Equal(responseString, word);
        }
    }
}
