using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;

namespace Kronos.Core.Tests.Requests
{
    public class ClearRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            var request = new ClearRequest();

            Assert.Equal(request.Type, RequestType.Clear);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            var request = new ClearRequest();

            byte[] packageBytes = SerializationUtils.Serialize(request);

            var requestFromBytes = SerializationUtils.Deserialize<ClearRequest>(packageBytes);

            Assert.NotNull(requestFromBytes);
            Assert.Equal(requestFromBytes.Type, request.Type);
        }
    }
}
