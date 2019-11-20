using System.Collections.Generic;
using Kronos.Core.Hashing;
using Kronos.Server.Storage;
using Xunit;

namespace Kronos.Server.Tests.Storage
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

        [Theory]
        [MemberData(nameof(ArgumentsData))]
        public void Equals_ReturnsFalse_WhenNullIsPassed(Key x, Key y, bool result)
        {
            bool comparisonResult = x.Equals(y);

            Assert.Equal(result, comparisonResult);
        }

        public static IEnumerable<object[]> ArgumentsData => new[]
        {
            new object[] { default(Key), new Key("key"), false },
            new object[] { new Key("key"), default(Key), false },
            new object[] { new Key("key"), new Key("key1"), false },
            new object[] { new Key("key"), new Key("key"), true},
        };
    }
}
