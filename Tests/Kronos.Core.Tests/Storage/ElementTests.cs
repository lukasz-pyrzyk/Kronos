//using System;
//using Google.Protobuf;
//using Kronos.Core.Storage;
//using Xunit;

//namespace Kronos.Core.Tests.Storage
//{
//    public class ElementTests
//    {
//        [Fact]
//        public void Ctor_AssignsValues_WithExpiry()
//        {
//            // Arrange
//            var data = new byte[0];
//            var expiry = DateTimeOffset.MaxValue;

//            // Act
//            var element = new Element(data, expiry);

//            // Assert
//            Assert.Equal(data, element.MemoryOwner.Memory);
//            Assert.Equal(expiry, element.ExpiryDate.Value);
//        }

//        [Fact]
//        public void Ctor_AssignsValues()
//        {
//            // Arrange
//            var data = new byte[0];

//            // Act
//            var element = new Element(data);

//            // Assert
//            Assert.Equal(data, element.MemoryOwner.Memory);
//            Assert.Null(element.ExpiryDate);
//        }

//        [Fact]
//        public void IsExpiring_ReturnsTrueWhenElementIsExpiring()
//        {
//            // Arrange
//            var element = new Element(new byte[0], DateTimeOffset.UtcNow);

//            // Act
//            bool expiring = element.IsExpiring;

//            // Assert
//            Assert.True(expiring);
//        }

//        [Fact]
//        public void IsExpiring_ReturnsFalseWhenElementIsExpiring()
//        {
//            // Arrange
//            var element = new Element(new byte[0]);

//            // Act
//            bool expiring = element.IsExpiring;

//            // Assert
//            Assert.False(expiring);
//        }

//        [Fact]
//        public void IsExpired_ReturnsTrueWhenKeyExpired_WithoutPassingDate()
//        {
//            // Arrange
//            var expiryDate = DateTimeOffset.MinValue;
//            var element = new Element(new byte[0], expiryDate);

//            // Act
//            bool expired = element.IsExpired();

//            // Assert
//            Assert.True(expired);
//        }

//        [Fact]
//        public void IsExpired_ReturnsTrueWhenKeyExpired_WithPassingDate()
//        {
//            // Arrange
//            var expiryDate = DateTimeOffset.MinValue;
//            var element = new Element(new byte[0], expiryDate);

//            // Act
//            bool expired = element.IsExpired(DateTimeOffset.UtcNow);

//            // Assert
//            Assert.True(expired);
//        }
//    }
//}
