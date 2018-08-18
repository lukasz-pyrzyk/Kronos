using System;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Serialization
{
    public class NoAllocBitConverterTests
    {
        [Fact]
        public void GetBytes_Short_ReturnsCorrectValue()
        {
            // Arrange
            short source = 5;

            // Act
            byte[] buffer = new byte[2];
            NoAllocBitConverter.GetBytes(source, buffer);
            short result = BitConverter.ToInt16(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_Int_ReturnsCorrectValue()
        {
            // Arrange
            int source = 5;

            // Act
            byte[] buffer = new byte[4];
            NoAllocBitConverter.GetBytes(source, buffer);
            int result = BitConverter.ToInt32(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_long_ReturnsCorrectValue()
        {
            // Arrange
            long source = 5;

            // Act
            byte[] buffer = new byte[8];
            NoAllocBitConverter.GetBytes(source, buffer);
            long result = BitConverter.ToInt64(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }
    }
}
