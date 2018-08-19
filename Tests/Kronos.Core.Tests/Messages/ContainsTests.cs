using FluentAssertions;
using Kronos.Core.Messages;
using Kronos.Core.Serialization;
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
            request.Type.Should().Be(RequestType.Contains);
            request.InternalRequest.Should().BeOfType<ContainsRequest>();
            var internalRequest = (ContainsRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
            internalRequest.Key.Span.GetString().Should().Be(key);
        }
    }
}
