using System;
using Google.Protobuf;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class ExpiringKeyTests
    {
        [Fact]
        public void Ctor_AssignsValues_WithExpiry()
        {
            // Arrange
            var key = new Key();
            var expiry = DateTime.MaxValue;

            // Act
            var element = new ExpiringKey(key, expiry);

            // Assert
            Assert.Equal(key, element.Key);
            Assert.Equal(expiry, element.ExpiryDate);
        }

        [Fact]
        public void IsExpiring_ReturnsTrueWhenElementIsExpiring()
        {
            // Arrange
            var element = new Element(ByteString.Empty, DateTime.UtcNow);

            // Act
            bool expiring = element.IsExpiring;

            // Assert
            Assert.True(expiring);
        }

        [Fact]
        public void IsExpired_ReturnsTrueWhenKeyExpired_WithoutPassingDate()
        {
            // Arrange
            var expiryDate = DateTime.MinValue;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            bool expired = element.IsExpired();

            // Assert
            Assert.True(expired);
        }

        [Fact]
        public void IsExpired_ReturnsTrueWhenKeyExpired_WithPassingDate()
        {
            // Arrange
            var expiryDate = DateTime.MinValue;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            bool expired = element.IsExpired(DateTime.UtcNow);

            // Assert
            Assert.True(expired);
        }

        [Fact]
        public void CompareTo_Returns_1_When_GivenIsBigger()
        {
            // Arrange
            var expiryDate = DateTime.Now;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            int result = element.CompareTo(new ExpiringKey(new Key("lorem ipsum"), expiryDate.AddDays(1)));

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void CompareTo_Returns_0_When_GivenIsEqual()
        {
            // Arrange
            var expiryDate = DateTime.Now;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            int result = element.CompareTo(new ExpiringKey(new Key("lorem ipsum"), expiryDate));

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CompareTo_Returns_Minus1_When_GivenIsEqual()
        {
            // Arrange
            var expiryDate = DateTime.Now;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            int result = element.CompareTo(new ExpiringKey(new Key("lorem ipsum"), expiryDate.AddDays(-1)));

            // Assert
            Assert.Equal(-1, result);
        }
    }
}
