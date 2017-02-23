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
            const string key = "lorem ipsum";
            byte[] data = new byte[2048];

            // Act
            var request = InsertRequest.New(key, data, expiry);

            // Assert
            Timestamp expectedTimestamp = expiry.HasValue ? Timestamp.FromDateTime(expiry.Value) : null;

            Assert.NotNull(request);
            Assert.Equal(request.Type, RequestType.Insert);
            Assert.NotNull(request.InsertRequest);
            Assert.Equal(request.InsertRequest.Key, key);
            Assert.Equal(request.InsertRequest.Data, data);
            Assert.Equal(request.InsertRequest.Expiry, expectedTimestamp);
        }

        [Fact]
        public void GetRequest_ReturnsNew()
        {
            // Arrange
            const string key = "lorem ipsum";

            // Act
            var request = GetRequest.New(key);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(request.Type, RequestType.Get);
            Assert.NotNull(request.GetRequest);
            Assert.Equal(request.GetRequest.Key, key);
        }

        [Fact]
        public void DeleteRequest_ReturnsNew()
        {
            // Arrange
            const string key = "lorem ipsum";

            // Act
            var request = DeleteRequest.New(key);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(request.Type, RequestType.Delete);
            Assert.NotNull(request.DeleteRequest);
            Assert.Equal(request.DeleteRequest.Key, key);
        }

        [Fact]
        public void CountRequest_ReturnsNew()
        {
            // Act
            var request = CountRequest.New();

            // Assert
            Assert.NotNull(request);
            Assert.Equal(request.Type, RequestType.Count);
            Assert.NotNull(request.CountRequest);
        }

        [Fact]
        public void ContainsRequest_ReturnsNew()
        {
            // Arrange
            const string key = "lorem ipsum";

            // Act
            var request = ContainsRequest.New(key);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(request.Type, RequestType.Contains);
            Assert.NotNull(request.ContainsRequest);
        }

        [Fact]
        public void ClearRequest_ReturnsNew()
        {
            // Act
            var request = ClearRequest.New();

            // Assert
            Assert.NotNull(request);
            Assert.Equal(request.Type, RequestType.Clear);
            Assert.NotNull(request.ClearRequest);
        }

        private static IEnumerable<object[]> ExpiryDates()
        {
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { null };
        }
    }
}
