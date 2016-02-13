using Kronos.Client.Command;
using Kronos.Client.Transfer;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Moq;
using Xunit;

namespace Kronos.Client.Tests.Command
{
    public class GetCommandTests
    {
        [Fact]
        public void ExecuteReturnsCorrectValue()
        {
            byte[] value = SerializationUtils.Serialize("lorem ipsum");
            var request = new GetRequest("masterKey");

            var communicationServiceMock = new Mock<ICommunicationService>();
            communicationServiceMock.Setup(x => x.SendToNode(request)).Returns(value);

            GetCommand command = new GetCommand(communicationServiceMock.Object, request);

            byte[] response = command.Execute();

            Assert.Equal(response, value);
            communicationServiceMock.Verify(x => x.SendToNode(It.IsAny<GetRequest>()), Times.Exactly(1));
        }
    }
}
