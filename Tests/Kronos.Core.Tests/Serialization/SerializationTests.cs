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

            using (var serialization = new SerializationStream(1024))
            {
                serialization.Write(content);
                serialization.Flush();

                var deserialization = new DeserializationStream(serialization.Memory);
                var fromBytes = deserialization.ReadString();

                fromBytes.Should().Be(content);
            }
        }
    }
}
