using System;
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

            var serialization = new SerializationStream(new Memory<byte>(new byte[1024]));

            serialization.Write(content);
            serialization.Flush();

            var deserialization = new DeserializationStream(serialization.MemoryWithLengthPrefix);
            deserialization.ReadInt(); // read request length
            var fromBytes = deserialization.ReadString();

            fromBytes.Should().Be(content);
        }
    }
}
