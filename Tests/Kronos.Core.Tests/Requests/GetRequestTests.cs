using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;

namespace Kronos.Core.Tests.Requests
{
    public class GetRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            GetRequest request = new GetRequest();

            Assert.Equal(request.Type, RequestType.Get);
        }

        [Fact]
        public void Ctor_CanAssingValues()
        {
            string key = "lorem ipsum";
            GetRequest request = new GetRequest(key);

            Assert.Equal(request.Key, key);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            GetRequest request = new GetRequest("key");

            byte[] packageBytes = SerializationUtils.Serialize(request);

            GetRequest requestFromBytes = SerializationUtils.Deserialize<GetRequest>(packageBytes);

            Assert.Equal(requestFromBytes.Type, request.Type);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }
    }
}
