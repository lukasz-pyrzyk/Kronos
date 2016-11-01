using System;
using System.Text;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using ProtoBuf;
using Xunit;

namespace Kronos.Core.Tests.Serialization
{
    public class SerializationutilsTests
    {
        [Fact]
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("key"), DateTime.Now);

            byte[] array = SerializationUtils.Serialize(request);
            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(array);

            Assert.Equal(requestFromBytes.Type, request.Type);
            Assert.Equal(requestFromBytes.Key, requestFromBytes.Key);
            Assert.Equal(requestFromBytes.ExpiryDate, requestFromBytes.ExpiryDate);
            Assert.Equal(requestFromBytes.Object, requestFromBytes.Object);
        }
    }
}
