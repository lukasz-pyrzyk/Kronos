using System;
using System.IO;
using System.Text;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using ProtoBuf;
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

            byte[] package;
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(ms, request, PrefixStyle.Fixed32);
                package = ms.ToArray();
            }

            InsertRequest requestFromBytes;
            using (MemoryStream ms = new MemoryStream(package))
            {
                requestFromBytes = Serializer.DeserializeWithLengthPrefix<InsertRequest>(ms, PrefixStyle.Fixed32);
            }

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
