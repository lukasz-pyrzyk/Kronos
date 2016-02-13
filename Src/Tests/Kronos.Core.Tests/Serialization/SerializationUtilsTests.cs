using System;
using System.Text;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Serialization
{
    public class SerializationutilsTests
    {
        [Fact]
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest(new CachedObject("key", Encoding.UTF8.GetBytes("key"), DateTime.Now));

            byte[] array = SerializationUtils.Serialize(request);
            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(array);

            Assert.Equal(requestFromBytes.RequestType, request.RequestType);
            Assert.Equal(requestFromBytes.ObjectToCache.Key, requestFromBytes.ObjectToCache.Key);
            Assert.Equal(requestFromBytes.ObjectToCache.ExpiryDate, requestFromBytes.ObjectToCache.ExpiryDate);
            Assert.Equal(requestFromBytes.ObjectToCache.Object, requestFromBytes.ObjectToCache.Object);
        }
    }
}
