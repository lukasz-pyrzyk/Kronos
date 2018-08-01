using FluentAssertions;
using Kronos.Core.Messages;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class ContainsTests
    {
        [Fact]
        public void CreatesCorrectMessage()
        {
            // Arrange
            const string key = "lorem ipsum";
            var auth = new Auth();

            // Act
            var request = ContainsRequest.New(key, auth);

            // Assert
            request.Should().NotBeNull();
            request.Auth.Should().Be(auth);
            request.InternalRequest.Should().BeOfType<ContainsRequest>();
            var internalRequest = (ContainsRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
            internalRequest.Key.Should().Be(key);
            internalRequest.Type.Should().Be(RequestType.Contains);
        }
    }
}
