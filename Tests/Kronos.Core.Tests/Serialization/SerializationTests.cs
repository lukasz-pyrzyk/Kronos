using FluentAssertions;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Serialization
{
    public class SerializationTests
    {
        [Fact]
        public void SerializeAndDeserializeString()
        {
            var content = "lorem ipsum";

            var serialization = new SerializationStream();
            serialization.Write(content);
            serialization.Flush();

            var deserialization = new DeserializationStream(serialization.Memory);
            serialization.Dispose();
            var fromBytes = deserialization.ReadString();

            fromBytes.Should().Be(content);
        }
    }
}
