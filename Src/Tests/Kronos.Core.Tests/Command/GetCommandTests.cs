using System.Text;
using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Moq;
using Xunit;

namespace Kronos.Core.Tests.Command
{
    public class GetCommandTests
    {
        [Fact]
        public void Execute_ReturnsCorrectValue()
        {
            byte[] value = Encoding.UTF8.GetBytes("lorem ipsum");
            var request = new GetRequest("masterKey");

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(value);

            GetCommand command = new GetCommand();

            byte[] response = command.Execute(communicationServiceMock.Object, request);

            Assert.Equal(response, value);
            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<GetRequest>()), Times.Exactly(1));
        }

        [Fact]
        public void Execute_ReturnsNullWhenServerHasReturnedNotFound()
        {
            byte[] value = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            var request = new GetRequest("masterKey");

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(value);

            GetCommand command = new GetCommand();

            byte[] response = command.Execute(communicationServiceMock.Object, request);

            Assert.Null(response);
            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<GetRequest>()), Times.Exactly(1));
        }

    }
}
