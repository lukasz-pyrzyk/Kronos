using System;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class ExpiryDateTests
    {
        [Theory]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void Ctor_AssignsValues(long ticks)
        {
            var expiryDate = new ExpiryDate(ticks);

            Assert.Equal(expiryDate.Ticks, ticks);
        }

        [Fact]
        public void CanCastFromDateTime()
        {
            // Arrange
            var date = DateTime.Now;
            // Act
            ExpiryDate expiryDate = date;

            // Assert
            Assert.Equal(expiryDate.Ticks, date.Ticks);
        }

        [Fact]
        public void CanCastToDateTime()
        {
            // Arrange
            var expiryDate = new ExpiryDate(DateTime.Now.Ticks);

            // Act
            DateTime date = expiryDate;

            // Assert
            Assert.Equal(date.Ticks, expiryDate.Ticks);
        }

        [Fact]
        public void ToString_ContainsInformationAboutDateTime()
        {
            // Arrange
            var date = DateTime.Now;
            ExpiryDate expiryDate = date;

            // Act
            string mesage = expiryDate.ToString();

            // Assert
            Assert.Contains(mesage, date.ToString("s"));
        }
    }
}
