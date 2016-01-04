using System;
using Kronos.Shared.Network.Model;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Shared.Tests.Model
{
    public class CachedObjectTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            string key = _fixture.Create<string>();
            byte[] objectToCache = _fixture.Create<byte[]>();
            DateTime expiryDate = _fixture.Create<DateTime>();

            CachedObject cachedObject = new CachedObject(key, objectToCache, expiryDate);

            Assert.Equal(key, cachedObject.Key);
            Assert.Equal(objectToCache, cachedObject.Object);
            Assert.Equal(expiryDate, cachedObject.ExpiryDate);
        }

        [Fact]
        public void CanSerializeCachedObjectToNetworkPackage()
        {
            CachedObject cachedObject = new CachedObject(_fixture.Create<string>(), _fixture.Create<byte[]>(), _fixture.Create<DateTime>());

            byte[] expectedNetworkPackage = cachedObject.SerializeNetworkPackage();

            Assert.NotNull(expectedNetworkPackage);
            Assert.NotEmpty(expectedNetworkPackage);
        }

        [Fact]
        public void CanDeserializeNetworkPackageToCachedObject()
        {
            CachedObject cachedObject = new CachedObject(_fixture.Create<string>(), _fixture.Create<byte[]>(), _fixture.Create<DateTime>());
            byte[] serializedObject = cachedObject.SerializeNetworkPackage();

            CachedObject deserializedObject = CachedObject.DeserializeNetworkPackage(serializedObject);

            Assert.Equal(cachedObject.Key, deserializedObject.Key);
            Assert.Equal(cachedObject.Object, deserializedObject.Object);
            Assert.Equal(cachedObject.ExpiryDate, deserializedObject.ExpiryDate);
        }
    }
}
