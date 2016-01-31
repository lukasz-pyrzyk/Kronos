using System;
using System.Text;
using BinaryFormatter;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class InsertRequestTests
    {
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

            BinaryConverter converter = new BinaryConverter();
            byte[] package = converter.Serialize(request);

            InsertRequest requestFromBytes = converter.Deserialize<InsertRequest>(package);

            Assert.Equal(requestFromBytes.ObjectToCache.Object, requestFromBytes.ObjectToCache.Object);
            Assert.Equal(requestFromBytes.ObjectToCache.ExpiryDate, requestFromBytes.ObjectToCache.ExpiryDate);
            Assert.Equal(requestFromBytes.ObjectToCache.Key, requestFromBytes.ObjectToCache.Key);

        }

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            CachedObject cachedObject = new CachedObject("key", new byte[5], DateTime.MaxValue);

            InsertRequest request = new InsertRequest(cachedObject);
            
            Assert.NotNull(request);
        }
    }
}
