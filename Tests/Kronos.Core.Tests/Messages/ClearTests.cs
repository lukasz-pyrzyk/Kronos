using FluentAssertions;
using Kronos.Core.Messages;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class ClearTests
    {
        [Fact]
        public void CreatesCorrectMessage()
        {
            // Arrange
            var auth = new Auth();

            // Act
            var request = ClearRequest.New(auth);

            // Assert
            request.Should().NotBeNull();
            request.Type.Should().Be(RequestType.Clear);
            request.Auth.Should().Be(auth);
            request.InternalRequest.Should().BeOfType<ClearRequest>();
            var internalRequest = (ClearRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
        }
    }
}
