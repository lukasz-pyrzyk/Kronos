using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Moq;
using Xunit;

namespace Kronos.Core.Tests.Command
{
    public class InsertCommandTests
    {
        [Theory]
        [InlineData(RequestStatusCode.Ok)]
        [InlineData(RequestStatusCode.Failed)]
        public void ExecuteReturnsCorrectValue(RequestStatusCode status)
        {
            var request = new InsertRequest();

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(SerializationUtils.Serialize(status));

            InsertCommand command = new InsertCommand();
            RequestStatusCode response = command.Execute(communicationServiceMock.Object, request);

            Assert.Equal(response, status);
            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<InsertRequest>()), Times.Exactly(1));
        }
    }
}
