using FluentAssertions;
using Kronos.Core.Messages;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class StatsTests
    {
        [Fact]
        public void CreatesCorrectMessage()
        {
            // Arrange
            var auth = new Auth();

            // Act
            var request = StatsRequest.New(auth);

            // Assert
            request.Should().NotBeNull();
            request.Auth.Should().Be(auth);
            request.InternalRequest.Should().BeOfType<StatsRequest>();
            var internalRequest = (StatsRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
            internalRequest.Type.Should().Be(RequestType.Stats);
        }
    }
}
