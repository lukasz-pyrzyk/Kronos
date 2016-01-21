using System;
using Kronos.Core.Model;
using Xunit;

namespace Kronos.Core.Tests.Model
{
    public class CachedObjectTests
    {
        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            string key = "key";
            byte[] objectToCache = new byte[5];
            DateTime expiryDate = DateTime.MaxValue;

            CachedObject cachedObject = new CachedObject(key, objectToCache, expiryDate);

            Assert.Equal(key, cachedObject.Key);
            Assert.Equal(objectToCache, cachedObject.Object);
            Assert.Equal(expiryDate, cachedObject.ExpiryDate);
        }
    }
}
