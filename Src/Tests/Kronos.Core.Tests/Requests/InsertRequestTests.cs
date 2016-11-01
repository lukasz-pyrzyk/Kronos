using System;
using System.Text;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class InsertRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            InsertRequest request = new InsertRequest();

            Assert.Equal(request.Type, RequestType.Insert);
        }

        [Fact]
        public void Ctor_CanAssingValues()
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
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("lorem ipsum"), DateTime.Now);

            byte[] packageBytes = SerializationUtils.Serialize(request);

            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(packageBytes);

            Assert.Equal(requestFromBytes.Object, request.Object);
            Assert.Equal(requestFromBytes.ExpiryDate, request.ExpiryDate);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }
    }
}
