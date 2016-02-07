using System.IO;
using Kronos.Core.Requests;
using ProtoBuf;
using Xunit;
using System.Linq;

namespace Kronos.Core.Tests.Requests
{
    public class GetRequestTests
    {
        [Fact]
        public void CanSerializeWithRequestType()
        {
            GetRequest request = new GetRequest("key");

            byte[] packageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, request);
                packageBytes = ms.ToArray();
            }

            RequestType type;
            using (MemoryStream ms = new MemoryStream(packageBytes.Take(sizeof(short)).ToArray()))
            {
                type = Serializer.Deserialize<RequestType>(ms);
            }

            Assert.Equal(type, request.RequestType);
        }


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
    }
}
