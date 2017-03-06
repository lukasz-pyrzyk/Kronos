using System;
using System.Collections.Generic;
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
            bool expired = metadata.IsExpired();

            // Assert
            Assert.Equal(expired, expected);
        }

        [Fact]
        public void IsExpired_ReturnsFalseWhenExpiryDateIsNul()
        {
            // Arrange
            var metadata = new Key("lorem ipsum");

            // Act
            bool expired = metadata.IsExpired();

            // Assert
            Assert.False(expired);
        }

        [Fact]
        public void CompareTo_ThrowsExceptionWhenKeyIsNotExpiring()
        {
            // Arrange
            var key = new Key("lorem ipsum");
            var keyToCompare = new Key("random", DateTime.UtcNow);

            // Act and assert
            Assert.Throws<InvalidOperationException>(() => key.CompareTo(keyToCompare));
        }

        [Fact]
        public void CompareTo_ThrowsExceptionWhenKeyToCompareIsNotExpiring()
        {
            // Arrange
            var key = new Key("lorem ipsum", DateTime.UtcNow);
            var keyToCompare = new Key("random");

            // Act and assert
            Assert.Throws<InvalidOperationException>(() => key.CompareTo(keyToCompare));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void CompareTo_CalculatesCorrectValues(DateTime expire, DateTime expireGiven, int expected)
        {
            // Arrange
            var key = new Key("lorem ipsum", expire);
            var keyToCompare = new Key("random", expireGiven);

            // Act
            int result = key.CompareTo(keyToCompare);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TestData()
        {
            var now = DateTime.UtcNow;
            yield return new object[] { now, now.AddDays(1), 1 };
            yield return new object[] { now, now, 0 };
            yield return new object[] { now.AddDays(1), now, -1 };
        }
    }
}
