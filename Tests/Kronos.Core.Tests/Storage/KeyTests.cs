using System;
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
            const string userKey = "value";
            var metadata = new Key(userKey);

            Assert.Equal(metadata.Value, userKey);
            Assert.Null(metadata.ExpiryDate);
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
            const string value = "value";
            DateTime expiryDate = DateTime.Now;
            var metadata = new Key(value, expiryDate);
            string message = metadata.ToString();

            Assert.Equal($"{value}|{expiryDate:s}", message);
        }
        
        [Theory]
        [InlineData(-5, true)]
        [InlineData(5, false)]
        public void IsExpired_CalculatesCorrectWhenExpiryDateSet(int days, bool expected)
        {
            // Arrange
            DateTime expiryDate = DateTime.UtcNow.AddDays(days);
            var metadata = new Key("lorem ipsum", expiryDate);

            // Act
            bool expired = metadata.IsExpired(DateTime.UtcNow);

            // Assert
            Assert.Equal(expired, expected);
        }

        [Fact]
        public void IsExpired_ReturnsFalseWhenExpiryDateIsNul()
        {
            // Arrange
            var metadata = new Key("lorem ipsum");

            // Act
            bool expired = metadata.IsExpired(DateTime.UtcNow);

            // Assert
            Assert.False(expired);
        }
    }
}
