using Kronos.Client.Command;
using Kronos.Client.Transfer;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Moq;
using Xunit;

namespace Kronos.Client.Tests.Command
{
    public class InsertCommandTests
    {
        [Theory]
        [InlineData(RequestStatusCode.Ok)]
        [InlineData(RequestStatusCode.Failed)]
        public void ExecuteReturnsCorrectValue(RequestStatusCode status)
        {
            var request = new InsertRequest();

            var communicationServiceMock = new Mock<ICommunicationService>();
            communicationServiceMock.Setup(x => x.SendToNode(request)).Returns(SerializationUtils.Serialize(status));

            InsertCommand command = new InsertCommand(communicationServiceMock.Object, request);

            RequestStatusCode response = command.Execute();

            Assert.Equal(response, status);
            communicationServiceMock.Verify(x => x.SendToNode(It.IsAny<InsertRequest>()), Times.Exactly(1));
        }
    }
}
