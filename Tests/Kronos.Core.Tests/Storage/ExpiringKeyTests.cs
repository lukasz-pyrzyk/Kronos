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
            var expiry = DateTimeOffset.MaxValue;

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
            var element = new Element(ByteString.Empty, DateTimeOffset.UtcNow);

            // Act
            bool expiring = element.IsExpiring;

            // Assert
            Assert.True(expiring);
        }

        [Fact]
        public void IsExpired_ReturnsTrueWhenKeyExpired_WithoutPassingDate()
        {
            // Arrange
            var expiryDate = DateTimeOffset.MinValue;
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
            var expiryDate = DateTimeOffset.MinValue;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            bool expired = element.IsExpired(DateTimeOffset.UtcNow);

            // Assert
            Assert.True(expired);
        }

        [Fact]
        public void CompareTo_Returns_1_When_GivenIsBigger()
        {
            // Arrange
            var expiryDate = DateTimeOffset.Now;
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
            var expiryDate = DateTimeOffset.Now;
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
            var expiryDate = DateTimeOffset.Now;
            var element = new ExpiringKey(new Key("lorem ipsum"), expiryDate);

            // Act
            int result = element.CompareTo(new ExpiringKey(new Key("lorem ipsum"), expiryDate.AddDays(-1)));

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void GetHashCode_ReturnsKeyHashcode()
        {
            // Arrange
            var key = new Key("lorem ipsum");
            var element = new ExpiringKey(key, default);

            // Act
            int hash = element.GetHashCode();

            // Assert
            Assert.Equal(key.GetHashCode(), hash);
        }

        [Fact]
        public void Equals_ReturnsFalse_IfTypeIsDifferent()
        {
            // Arrange
            var element = new ExpiringKey(new Key(""), default);

            // Act
            bool equal = element.Equals(new object());

            // Assert
            Assert.False(equal);
        }

        [Fact]
        public void Equals_ReturnsTrue_IfElementIsTheSame()
        {
            // Arrange
            var element = new ExpiringKey(new Key(""), default);

            // Act
            bool equal = element.Equals(element);

            // Assert
            Assert.True(equal);
        }

        [Fact]
        public void Equals_ReturnsTrue_IfElementIsTheSame_WhenPassedIsObject()
        {
            // Arrange
            var element = new ExpiringKey(new Key(""), default);

            // Act
            bool equal = element.Equals((object)element);

            // Assert
            Assert.True(equal);
        }

        [Fact]
        public void ToString_ContainsInformationAboutNameAndExpiry()
        {
            // Arrange
            const string key = "lorem";
            var time = DateTimeOffset.Now;
            var element = new ExpiringKey(new Key(key), time);

            // Act
            string value = element.ToString();

            // Assert
            Assert.Contains(key, value);
            Assert.Contains(time.ToString("g"), value);
        }
    }
}
