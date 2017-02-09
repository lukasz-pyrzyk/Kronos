using System;
using System.Collections;
using System.Collections.Generic;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class NodeComparerTests
    {
        [Theory]
        [MemberData(nameof(ArgumentsData))]
        public void Equals_ReturnsFalse_WhenNullIsPassed(NodeMetatada x, NodeMetatada y, bool result)
        {
            IEqualityComparer<NodeMetatada> comparer = new KeyComperer();
            bool comparisonResult = comparer.Equals(x, y);

            Assert.Equal(result, comparisonResult);
        }

        [Fact]
        public void GetHashcode_ReturnsKeyHashcode()
        {
            string key = "key";
            NodeMetatada metatada = new NodeMetatada(key);

            Assert.Equal(key.GetHashCode(), metatada.GetHashCode());
        }

        public static IEnumerable<object[]> ArgumentsData => new[]
        {
            new object[] { null, null, false },
            new object[] { null, new NodeMetatada("key"), false },
            new object[] { new NodeMetatada("key"), null, false },
            new object[] { new NodeMetatada("key"), new NodeMetatada("key1"), false },
            new object[] { new NodeMetatada("key"), new NodeMetatada("key"), true},
        };
    }
}
