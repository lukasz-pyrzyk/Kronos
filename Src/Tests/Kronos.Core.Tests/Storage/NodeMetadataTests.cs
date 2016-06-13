using System;
using System.Collections.Generic;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class NodeMetadataTests
    {
        [Theory]
        [MemberData(nameof(ArgumentsData))]
        public void Ctor_AssignsProperties(string key, DateTime expiryDate)
        {
            var metadata = new NodeMetatada(key, expiryDate);

            Assert.Equal(key, metadata.Key);
            Assert.Equal(expiryDate, metadata.ExpiryDate);
        }

        [Fact]
        public void ToString_ContainsInformationAboutKeyAndExpiry()
        {
            string key = "key";
            DateTime expiryDate = DateTime.Now;
            var metadata = new NodeMetatada(key, expiryDate);
            string message = metadata.ToString();

            Assert.Equal($"{key}|{expiryDate:o}", message);
        }

        [Fact]
        public void HashCode_ReturnsHashCodeFromKey()
        {
            string key = "key";
            var metadata = new NodeMetatada(key);

            Assert.Equal(key.GetHashCode(), metadata.GetHashCode());
        }


        public static IEnumerable<object[]> ArgumentsData => new[]
        {
            new object[] { "key", DateTime.Now },
            new object[] { "key", null }
        };
    }
}
