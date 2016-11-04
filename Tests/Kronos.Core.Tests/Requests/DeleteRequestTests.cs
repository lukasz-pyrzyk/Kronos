using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;

namespace Kronos.Core.Tests.Requests
{
    public class DeleteRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            DeleteRequest request = new DeleteRequest();

            Assert.Equal(request.Type, RequestType.Delete);
        }

        [Fact]
        public void Ctor_CanAssingValues()
        {
            string key = "lorem ipsum";
            DeleteRequest request = new DeleteRequest(key);

            Assert.Equal(request.Key, key);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            DeleteRequest request = new DeleteRequest("key");

            byte[] packageBytes = SerializationUtils.Serialize(request);

            DeleteRequest requestFromBytes = SerializationUtils.Deserialize<DeleteRequest>(packageBytes);

            Assert.Equal(requestFromBytes.Type, request.Type);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }
    }
}
