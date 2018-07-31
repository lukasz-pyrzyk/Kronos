using FluentAssertions;
using Kronos.Core.Messages;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class CountTests
    {
        [Fact]
        public void CreatesCorrectMessage()
        {
            // Arrange
            var auth = new Auth();

            // Act
            var request = CountRequest.New(auth);

            // Assert
            request.Should().NotBeNull();
            request.Type.Should().Be(RequestType.Count);
            request.Auth.Should().Be(auth);
            request.InternalRequest.Should().BeOfType<CountRequest>();
            var internalRequest = (CountRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
        }
    }
}
