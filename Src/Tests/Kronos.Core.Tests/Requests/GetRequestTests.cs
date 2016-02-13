using Kronos.Core.Requests;
using Xunit;
using System.Linq;
using Kronos.Core.Serialization;

namespace Kronos.Core.Tests.Requests
{
    public class GetRequestTests
    {
        [Fact]
        public void ContainsCorrectRequestType()
        {
            GetRequest request = new GetRequest();

            Assert.Equal(request.RequestType, RequestType.GetRequest);
        }

        [Fact]
        public void CanAssignCorrectValuesByConstructor()
        {
            string key = "lorem ipsum";
            GetRequest request = new GetRequest(key);

            Assert.Equal(request.Key, key);
        }

        [Fact]
        public void CanSerializeWithRequestType()
        {
            GetRequest request = new GetRequest("key");

            byte[] packageBytes = SerializationUtils.Serialize(request);
            RequestType type = SerializationUtils.Deserialize<RequestType>(packageBytes.Take(sizeof(short)).ToArray());

            Assert.Equal(type, request.RequestType);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            GetRequest request = new GetRequest("key");

            byte[] packageBytes = SerializationUtils.Serialize(request);

            GetRequest requestFromBytes = SerializationUtils.Deserialize<GetRequest>(packageBytes);

            Assert.Equal(requestFromBytes.RequestType, requestFromBytes.RequestType);
            Assert.Equal(requestFromBytes.Key, requestFromBytes.Key);
        }
    }
}
