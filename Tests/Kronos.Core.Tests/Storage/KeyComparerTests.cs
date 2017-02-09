using System.Collections.Generic;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class KeyComparerTests
    {
        [Theory]
        [MemberData(nameof(ArgumentsData))]
        public void Equals_ReturnsFalse_WhenNullIsPassed(Key x, Key y, bool result)
        {
            IEqualityComparer<Key> comparer = new KeyComperer();
            bool comparisonResult = comparer.Equals(x, y);

            Assert.Equal(result, comparisonResult);
        }

        [Fact]
        public void GetHashcode_ReturnsKeyHashcode()
        {
            string value = "key";
            Key metatada = new Key(value);

            Assert.Equal(value.GetHashCode(), metatada.GetHashCode());
        }

        public static IEnumerable<object[]> ArgumentsData => new[]
        {
            new object[] { default(Key), default(Key), false },
            new object[] { default(Key), new Key("key"), false },
            new object[] { new Key("key"), default(Key), false },
            new object[] { new Key("key"), new Key("key1"), false },
            new object[] { new Key("key"), new Key("key"), true},
        };
    }
}
