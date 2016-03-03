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
            string key = "key";
            byte[] serializedObject = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiryDate = DateTime.Today;

            InsertRequest request = new InsertRequest(key, serializedObject, expiryDate);

            Assert.NotNull(request);
            Assert.Equal(request.Key, key);
            Assert.Equal(request.Object, serializedObject);
            Assert.Equal(request.ExpiryDate, expiryDate);
        }

        [Fact]
        public void CanSerializeWithRequestType()
        {
            InsertRequest request = new InsertRequest("key", new byte[0], DateTime.Now);

            byte[] packageBytes = SerializationUtils.Serialize(request);
            RequestType type = SerializationUtils.Deserialize<RequestType>(packageBytes.Take(sizeof(short)).ToArray());

            Assert.Equal(type, request.RequestType);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest
            {
                Object = Encoding.UTF8.GetBytes("lorem ipsum"),
                ExpiryDate = DateTime.Now,
                Key = "key"
            };

            byte[] packageBytes = SerializationUtils.Serialize(request);

            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(packageBytes);

            Assert.Equal(requestFromBytes.Object, requestFromBytes.Object);
            Assert.Equal(requestFromBytes.ExpiryDate, requestFromBytes.ExpiryDate);
            Assert.Equal(requestFromBytes.Key, requestFromBytes.Key);
        }
    }
}
