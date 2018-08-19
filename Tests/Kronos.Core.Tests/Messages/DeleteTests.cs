using FluentAssertions;
using Kronos.Core.Messages;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class DeleteTests
    {
        [Fact]
        public void CreatesCorrectMessage()
        {
            // Arrange
            const string key = "lorem ipsum";
            var auth = new Auth();

            // Act
            var request = DeleteRequest.New(key, auth);

            // Assert
            request.Should().NotBeNull();
            request.Auth.Should().Be(auth);
            request.Type.Should().Be(RequestType.Delete);
            request.InternalRequest.Should().BeOfType<DeleteRequest>();
            var internalRequest = (DeleteRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
            internalRequest.Key.Span.GetString().Should().Be(key);
        }
    }
}
