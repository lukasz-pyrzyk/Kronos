using Kronos.Core.Hashing;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class KeyTests
    {
        [Fact]
        public void Ctor_AssignsProperties()
        {
            // Arrange
            const string userKey = "value";

            // Act
            var metadata = new Key(userKey);

            // Assert
            Assert.Equal(metadata.Name, userKey);
        }

        [Fact]
        public void GetHashcode_ReturnsKeyHashcode()
        {
            // Arrange
            const string value = "key";
            int expectedHash = Hasher.Hash(value);

            // Act
            Key metatada = new Key(value);
            int hash = metatada.GetHashCode();

            // Assert
            Assert.Equal(expectedHash, hash);
        }

        [Fact]
        public void ToString_ContainsInformationAboutKeyAndExpiry()
        {
            // Arrange
            const string value = "value";
            var metadata = new Key(value);

            // Act
            string message = metadata.ToString();


            // Assert
            Assert.Equal(value, message);
        }
    }
}
