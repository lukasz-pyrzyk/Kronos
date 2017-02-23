using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Kronos.Core.Messages;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class RequestExtensionsTests
    {
        [Theory]
        [MemberData(nameof(ExpiryDates))]
        public void InsertRequest_ReturnsNew(DateTime? expiry)
        {
            // Arrange
            string key = "lorem ipsum";
            byte[] data = new byte[2048];

            // Act
            var request = InsertRequest.New(key, data, expiry);

            // Assert
            Timestamp expectedTimestamp = expiry.HasValue ? Timestamp.FromDateTime(expiry.Value) : null;
            Assert.NotNull(request);
            Assert.NotNull(request.InsertRequest);
            Assert.Equal(request.InsertRequest.Key, key);
            Assert.Equal(request.InsertRequest.Data, data);
            Assert.Equal(request.InsertRequest.Expiry, expectedTimestamp);
        }

        private static IEnumerable<object[]> ExpiryDates()
        {
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { null };
        }
    }
}
