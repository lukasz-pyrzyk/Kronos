using System;
using System.Collections.Generic;
using Kronos.Core.Hashing;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class KeyTests
    {
        private ReadOnlyMemory<byte> _key = "value".GetMemory();
        private ReadOnlyMemory<byte> _key1 = "value1".GetMemory();

        [Fact]
        public void Ctor_AssignsProperties()
        {
            // Act
            var metadata = new Key(_key);

            // Assert
            Assert.Equal(metadata.Name, _key);
        }

        [Fact]
        public void GetHashcode_ReturnsKeyHashcode()
        {
            // Arrange
            int expectedHash = Hasher.Hash(_key.Span);

            // Act
            Key metatada = new Key(_key);
            int hash = metatada.GetHashCode();

            // Assert
            Assert.Equal(expectedHash, hash);
        }

        [Theory]
        [MemberData(nameof(ArgumentsData))]
        public void Equals_ReturnsFalse_WhenNullIsPassed(Key x, Key y, bool result)
        {
            bool comparisonResult = x.Equals(y);

            Assert.Equal(result, comparisonResult);
        }

        public static IEnumerable<object[]> ArgumentsData ()
        {
            var first = Guid.NewGuid().ToString().GetMemory();
            var second = Guid.NewGuid().ToString().GetMemory();

            yield return new object[] {default(Key), new Key(first), false};
            yield return new object[] {new Key(first), default(Key), false};
            yield return new object[] {new Key(first), new Key(second), false};
            yield return new object[] {new Key(first), new Key(first), true};
        }
    }
}
