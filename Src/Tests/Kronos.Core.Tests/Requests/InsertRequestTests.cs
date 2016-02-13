using System;
using System.Linq;
using System.Text;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class InsertRequestTests
    {
        [Fact]
        public void ContainsCorrectRequestType()
        {
            InsertRequest request = new InsertRequest();

            Assert.Equal(request.RequestType, RequestType.InsertRequest);
        }

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            CachedObject cachedObject = new CachedObject("key", new byte[5], DateTime.MaxValue);
            InsertRequest request = new InsertRequest(cachedObject);

            Assert.NotNull(request);
        }

        [Fact]
        public void CanSerializeWithRequestType()
        {
            InsertRequest request = new InsertRequest(new CachedObject("key", new byte[0], DateTime.Now));

            byte[] packageBytes = SerializationUtils.Serialize(request);
            RequestType type = SerializationUtils.Deserialize<RequestType>(packageBytes.Take(sizeof(short)).ToArray());

            Assert.Equal(type, request.RequestType);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest
            {
                ObjectToCache = new CachedObject
                {
                    Object = Encoding.UTF8.GetBytes("lorem ipsum"),
                    ExpiryDate = DateTime.Now,
                    Key = "key"
                }
            };

            byte[] packageBytes = SerializationUtils.Serialize(request);

            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(packageBytes);

            Assert.Equal(requestFromBytes.ObjectToCache.Object, requestFromBytes.ObjectToCache.Object);
            Assert.Equal(requestFromBytes.ObjectToCache.ExpiryDate, requestFromBytes.ObjectToCache.ExpiryDate);
            Assert.Equal(requestFromBytes.ObjectToCache.Key, requestFromBytes.ObjectToCache.Key);
        }
    }
}
