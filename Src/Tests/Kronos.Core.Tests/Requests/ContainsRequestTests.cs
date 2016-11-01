using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class ContainsRequestTests
    {

        [Fact]
        public void Ctor_AssignsProperties()
        {
            string key = "lorem ipsum";
            var request = new ContainsRequest(key);

            Assert.Equal(key, request.Key);
        }

        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            var request = new ContainsRequest();

            Assert.Equal(request.Type, RequestType.Contains);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            string key = "lorem ipsum";
            var request = new ContainsRequest(key);

            byte[] packageBytes = SerializationUtils.Serialize(request);

            ContainsRequest requestFromBytes = SerializationUtils.Deserialize<ContainsRequest>(packageBytes);

            Assert.NotNull(requestFromBytes);
            Assert.Equal(requestFromBytes.Key, key);
        }
    }
}
