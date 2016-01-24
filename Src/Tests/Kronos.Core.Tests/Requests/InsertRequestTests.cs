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
            DateTime date = new DateTime();

            InsertRequest request = new InsertRequest(new CachedObject(key, objectToStore, date));

            byte[] array = request.Serialize();
            
            // TODO deserialize and compare
        }
    }
}
