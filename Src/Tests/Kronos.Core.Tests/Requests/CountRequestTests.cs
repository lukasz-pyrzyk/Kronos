using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class CountRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            var request = new CountRequest();

            Assert.Equal(request.Type, RequestType.Count);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            var request = new CountRequest();

            byte[] packageBytes = SerializationUtils.Serialize(request);

            CountRequest requestFromBytes = SerializationUtils.Deserialize<CountRequest>(packageBytes);

            Assert.NotNull(requestFromBytes);
        }
    }
}
