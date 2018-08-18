using FluentAssertions;
using Kronos.Core.Messages;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class ClearRequestTests
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
            request.Auth.Should().Be(auth);
            request.Type.Should().Be(RequestType.Clear);
            request.InternalRequest.Should().BeOfType<ClearRequest>();
            var internalRequest = (ClearRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
        }

        [Fact]
        public void CanBeSerializedAndDeserialized()
        {
            // Arrange
            var auth = new Auth();

            // Act
            var request = ClearRequest.New(auth);

            // Assert
            using (var s = new SerializationStream(1024))
            {
                request.Write(s);
            }
        }
    }
}
