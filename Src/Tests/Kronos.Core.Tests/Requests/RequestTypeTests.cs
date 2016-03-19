using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestTypeTests
    {
        [Theory]
        [InlineData(RequestType.Insert)]
        [InlineData(RequestType.Get)]
        public void CanSerializeAndDeserialize(RequestType type)
        {
            byte[] package = SerializationUtils.Serialize(type);

            RequestType typeFromBytes = SerializationUtils.Deserialize<RequestType>(package);

            Assert.Equal(type, typeFromBytes);
        }
    }
}
