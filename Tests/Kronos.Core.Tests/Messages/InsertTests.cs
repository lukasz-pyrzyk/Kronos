﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Kronos.Core.Messages;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Messages
{
    public class InsertTests
    {
        [Theory]
        [MemberData(nameof(ExpiryDates))]
        public void CreatesCorrectMessage(DateTimeOffset? expiry)
        {
            // Arrange
            const string key = "lorem ipsum";
            byte[] data = new byte[2048];

            // Act
            var auth = new Auth();
            var request = InsertRequest.New(key, data, expiry, auth);

            // Assert
            request.Should().NotBeNull();
            request.Auth.Should().Be(auth);
            request.InternalRequest.Should().BeOfType<InsertRequest>();
            request.Type.Should().Be(RequestType.Insert);

            var internalRequest = (InsertRequest)request.InternalRequest;
            internalRequest.Should().NotBeNull();
            internalRequest.Key.Span.GetString().Should().Be(key);
            internalRequest.Expiry.Should().Be(expiry);
            internalRequest.Data.ToArray().Should().Equal(data);
        }

        public static IEnumerable<object[]> ExpiryDates()
        {
            yield return new object[] { DateTimeOffset.UtcNow };
            yield return new object[] { null };
        }
    }
}
