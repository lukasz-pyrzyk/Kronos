using System;
using System.Text;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class InsertRequestTests
    {
        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            CachedObject cachedObject = new CachedObject("key", new byte[5], DateTime.MaxValue);

            InsertRequest request = new InsertRequest(cachedObject);
            
            Assert.NotNull(request);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            string key = "key";
            byte[] objectToStore = Encoding.UTF8.GetBytes("object");
            DateTime date = DateTime.Now;

            InsertRequest request = new InsertRequest(new CachedObject(key, objectToStore, date));

            byte[] array = request.Serialize();

            InsertRequest requestFromBytes = InsertRequest.Deserialize(array);

            Assert.NotNull(requestFromBytes);
            Assert.NotNull(requestFromBytes.ObjectToCache);
            Assert.Equal(requestFromBytes.ObjectToCache.Key, key);
            Assert.Equal(requestFromBytes.ObjectToCache.Object, objectToStore);
            Assert.Equal(requestFromBytes.ObjectToCache.ExpiryDate.Ticks, date.Ticks);
        }
    }
}
