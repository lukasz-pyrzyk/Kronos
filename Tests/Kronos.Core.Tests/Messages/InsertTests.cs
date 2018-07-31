using System;
using System.Collections.Generic;
using FluentAssertions;
using Kronos.Core.Messages;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class InsertTests
    {
        [Theory]
        [MemberData(nameof(ExpiryDates))]
        public void CreatesCorrectMessage(DateTime? expiry)
        {
            // Arrange
            const string key = "lorem ipsum";
            byte[] data = new byte[2048];

            // Act
            var auth = new Auth();
            var request = InsertRequest.New(key, data, expiry, auth);

            // Assert
            request.Should().NotBeNull();
            request.Type.Should().Be(RequestType.Insert);
            request.Auth.Should().Be(auth);
            request.InternalRequest.Should().BeOfType<InsertRequest>();
            var internalRequest = (InsertRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
            internalRequest.Key.Should().Be(key);
            internalRequest.Expiry.Should().Be(expiry);
            internalRequest.Data.Should().Equal(data);
        }

        public static IEnumerable<object[]> ExpiryDates()
        {
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { null };
        }
    }
}
