using System;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Serialization
{
    public class NoAllocBitConverterTests
    {
        [Fact]
        public void GetBytes_Char_ReturnsCorrectValue()
        {
            // Arrange
            char source = (char)5;

            // Act
            byte[] buffer = new byte[2];
            NoAllocBitConverter.GetBytes(source, buffer);
            char result = BitConverter.ToChar(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_Short_ReturnsCorrectValue()
        {
            // Arrange
            short source = (short)5;

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
            int source = (int)5;

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
            long source = (long)5;

            // Act
            byte[] buffer = new byte[8];
            NoAllocBitConverter.GetBytes(source, buffer);
            long result = BitConverter.ToInt64(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_Ushort_ReturnsCorrectValue()
        {
            // Arrange
            ushort source = (ushort)5;

            // Act
            byte[] buffer = new byte[2];
            NoAllocBitConverter.GetBytes(source, buffer);
            ushort result = BitConverter.ToUInt16(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_Uint_ReturnsCorrectValue()
        {
            // Arrange
            uint source = (uint)5;

            // Act
            byte[] buffer = new byte[4];
            NoAllocBitConverter.GetBytes(source, buffer);
            uint result = BitConverter.ToUInt32(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_Ulong_ReturnsCorrectValue()
        {
            // Arrange
            ulong source = (ulong)5;

            // Act
            byte[] buffer = new byte[8];
            NoAllocBitConverter.GetBytes(source, buffer);
            ulong result = BitConverter.ToUInt64(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_float_ReturnsCorrectValue()
        {
            // Arrange
            float source = (float)5;

            // Act
            byte[] buffer = new byte[4];
            NoAllocBitConverter.GetBytes(source, buffer);
            float result = BitConverter.ToSingle(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }

        [Fact]
        public void GetBytes_Double_ReturnsCorrectValue()
        {
            // Arrange
            double source = (double)5;

            // Act
            byte[] buffer = new byte[8];
            NoAllocBitConverter.GetBytes(source, buffer);
            double result = BitConverter.ToDouble(buffer, 0);

            // Assert
            Assert.Equal(source, result);
        }
    }
}
