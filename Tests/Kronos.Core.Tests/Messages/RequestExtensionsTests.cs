using System;
using System.Collections.Generic;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
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
            Auth auth = new Auth();
            var request = InsertRequest.New(key, data, expiry, auth);

            // Assert
            Timestamp expectedTimestamp = expiry.HasValue ? Timestamp.FromDateTime(expiry.Value) : null;

            Assert.NotNull(request);
            Assert.Equal(RequestType.Insert, request.Type);
            Assert.NotNull(request.InsertRequest);
            Assert.Equal(request.InsertRequest.Key, key);
            Assert.Equal(request.InsertRequest.Data, data);
            Assert.Equal(request.InsertRequest.Expiry, expectedTimestamp);
            Assert.Equal(auth, request.Auth);
        }

        [Fact]
        public void InsertRequest_ThrowsExceptionWhenContentIsTooBig()
        {
            // Arrange
            const string key = "lorem ipsum";
            byte[] data = new byte[Settings.MaxRequestSize + 1];

            // Act
            Action action = () => InsertRequest.New(key, data, DateTime.Now, new Auth());

            // Assert

            action.Should().Throw<KronosException>().WithMessage($"Content size exceeded. Maximum value is {Settings.MaxRequestSize} bytes");
        }

        [Fact]
        public void GetRequest_ReturnsNew()
        {
            // Arrange
            const string key = "lorem ipsum";
            Auth auth = new Auth();

            // Act
            var request = GetRequest.New(key, auth);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(RequestType.Get, request.Type);
            Assert.NotNull(request.GetRequest);
            Assert.Equal(request.GetRequest.Key, key);
            Assert.Equal(auth, request.Auth);
        }

        [Fact]
        public void DeleteRequest_ReturnsNew()
        {
            // Arrange
            const string key = "lorem ipsum";
            Auth auth = new Auth();

            // Act
            var request = DeleteRequest.New(key, auth);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(RequestType.Delete, request.Type);
            Assert.NotNull(request.DeleteRequest);
            Assert.Equal(request.DeleteRequest.Key, key);
            Assert.Equal(auth, request.Auth);
        }

        [Fact]
        public void CountRequest_ReturnsNew()
        {
            // Act
            Auth auth = new Auth();
            var request = CountRequest.New(auth);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(RequestType.Count, request.Type);
            Assert.NotNull(request.CountRequest);
            Assert.Equal(auth, request.Auth);
        }

        [Fact]
        public void ContainsRequest_ReturnsNew()
        {
            // Arrange
            const string key = "lorem ipsum";
            Auth auth = new Auth();

            // Act
            var request = ContainsRequest.New(key, auth);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(RequestType.Contains, request.Type);
            Assert.NotNull(request.ContainsRequest);
            Assert.Equal(auth, request.Auth);
        }

        [Fact]
        public void ClearRequest_ReturnsNew()
        {
            // Arrange
            Auth auth = new Auth();


            // Act
            var request = ClearRequest.New(auth);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(RequestType.Clear, request.Type);
            Assert.NotNull(request.ClearRequest);
            Assert.Equal(auth, request.Auth);
        }

        public static IEnumerable<object[]> ExpiryDates()
        {
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { null };
        }
    }
}
