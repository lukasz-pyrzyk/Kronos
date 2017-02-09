using System;
using System.Collections.Generic;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class KeyTests
    {
        [Theory]
        [MemberData(nameof(ArgumentsData))]
        public void Ctor_AssignsProperties(string value, DateTime expiryDate)
        {
            var metadata = new Key(value, expiryDate);

            Assert.Equal(value, metadata.Value);
            Assert.Equal(expiryDate.Ticks, metadata.ExpiryDate.Ticks);
        }

        [Fact]
        public void ToString_ContainsInformationAboutKeyAndExpiry()
        {
            string value = "value";
            DateTime expiryDate = DateTime.Now;
            var metadata = new Key(value, expiryDate);
            string message = metadata.ToString();

            Assert.Equal($"{value}|{expiryDate:s}", message);
        }

        [Fact]
        public void HashCode_ReturnsHashCodeFromKey()
        {
            string value = "value";
            var metadata = new Key(value);

            Assert.Equal(value.GetHashCode(), metadata.GetHashCode());
        }


        public static IEnumerable<object[]> ArgumentsData => new[]
        {
            new object[] { "value", DateTime.Now },
            new object[] { "value", null }
        };
    }
}
