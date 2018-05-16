using System;
using System.Linq;
using FluentAssertions;
using Kronos.Core.Configuration;
using Xunit;
using BufferedStream = Kronos.Core.Pooling.BufferedStream;

namespace Kronos.Core.Tests.Pooling
{
    public class BufferedStreamTests
    {
        [Fact]
        public void Rawbytes_ReturnsCollection()
        {
            // Arrange
            var stream = NewStream();

            // Act
            byte[] bytes = stream.RawBytes;

            // Assert
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void IsClean_IsTrue_WhenIsNew()
        {
            // Arrange
            var stream = NewStream();

            // Act
            bool isClean = stream.IsClean;

            // Assert
            Assert.True(isClean);
        }

        [Fact]
        public void IsClean_IsFalse_WhenDataIsWritten()
        {
            // Arrange
            var stream = NewStream();
            byte[] data = new byte[1024];
            stream.Write(data, 0, data.Length);

            // Act
            bool isClean = stream.IsClean;

            // Assert
            Assert.False(isClean);
        }

        [Fact]
        public void IsClean_IsTrue_WhenDataIsWrittenAndStreamIsClean()
        {
            // Arrange
            var stream = NewStream();
            byte[] data = new byte[1024];
            stream.Write(data, 0, data.Length);
            stream.Clean();

            // Act
            bool isClean = stream.IsClean;

            // Assert
            Assert.True(isClean);
        }

        [Fact]
        public void CanRead_IsTrue()
        {
            // Arrange
            var stream = NewStream();

            // Act
            bool value = stream.CanRead;

            // Assert
            Assert.True(value);
        }

        [Fact]
        public void CanWrite_IsTrue()
        {
            // Arrange
            var stream = NewStream();

            // Act
            bool value = stream.CanWrite;

            // Assert
            Assert.True(value);
        }

        [Fact]
        public void CanSeek_IsFalse()
        {
            // Arrange
            var stream = NewStream();

            // Act
            bool value = stream.CanSeek;

            // Assert
            Assert.False(value);
        }

        [Fact]
        public void Length_IsMovedByReservedBytesForSize()
        {
            // Arrange
            var stream = NewStream();

            // Act and assert
            Assert.Equal(stream.Position, sizeof(int));
        }

        [Fact]
        public void Position_IsMovedAfterWriting()
        {
            // Arrange
            var stream = NewStream();

            // Act and assert
            Assert.Equal(stream.Position, sizeof(int));
        }

        [Fact]
        public void Length_IsMovedAfterWriting()
        {
            // Arrange
            var stream = NewStream();
            int position = (int)stream.Position;
            byte[] data = new byte[1024];

            // Act
            stream.Write(data, 0, data.Length);

            // Assert
            Assert.Equal(stream.Position, position + data.Length);
        }

        [Fact]
        public void Position_IsMovedByReservedBytesForSize()
        {
            // Arrange
            var stream = NewStream();
            int defaultPosition = (int)stream.Position;
            byte[] data = new byte[1024];

            // Act
            stream.Write(data, 0, data.Length);

            // Assert
            Assert.Equal(stream.Position, defaultPosition + data.Length);
        }

        [Fact]
        public void Flush_DoesNotThrow()
        {
            // Arrange
            var stream = NewStream();

            // Act
            var ex = Record.Exception(() => stream.Flush());

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void Flush_WritesSizeToTheHeader()
        {
            // Arrange
            var stream = NewStream();

            // Act
            var data = new byte[1024];
            stream.Write(data, 0, data.Length);

            stream.Flush();
            stream.SetPosition(0);

            // Assert
            byte[] bytesForTheSize = new byte[sizeof(int)];
            Array.Copy(stream.RawBytes, 0, bytesForTheSize, 0, bytesForTheSize.Length);
            int sizeInHeader = BitConverter.ToInt32(bytesForTheSize, 0);
            sizeInHeader.Should().Be(data.Length);
        }

        [Fact]
        public void Clean_ResetsPositions()
        {
            // Arrange
            var stream = NewStream();
            int length = (int)stream.Length;
            int position = (int)stream.Position;
            byte[] data = new byte[1024];
            stream.Write(data, 0, data.Length);

            // Act
            stream.Clean();

            // Assert
            Assert.Equal(stream.Position, position);
            Assert.Equal(stream.Length, length);
        }

        [Fact]
        public void SetLength_ThrowsNotSupportedException()
        {
            // Arrange
            var stream = NewStream();

            // Act
            var ex = Record.Exception(() => stream.SetLength(0));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Read_ReadsDataFromStream_WhenCountIsTheSame()
        {
            // Arrange
            var stream = NewStream();
            byte[] data = new byte[1024];
            new Random().NextBytes(data);

            // Act
            stream.Write(data, 0, data.Length);
            stream.SetPosition(0);
            byte[] receivedBytes = new byte[data.Length];
            stream.Read(receivedBytes, 0, receivedBytes.Length);

            // Assert
            Assert.Equal(data, receivedBytes);
        }

        [Fact]
        public void Read_ReadsDataFromStream_WhenCountIsSmaller()
        {
            // Arrange
            var stream = NewStream();
            byte[] data = new byte[1024];
            new Random().NextBytes(data);

            // Act
            stream.Write(data, 0, data.Length);
            stream.SetPosition(0);
            byte[] receivedBytes = new byte[data.Length - 1];
            stream.Read(receivedBytes, 0, receivedBytes.Length);

            // Assert
            Assert.Equal(data.Take(receivedBytes.Length), receivedBytes);
        }

        private static BufferedStream NewStream()
        {
            return new BufferedStream(Settings.MaxRequestSize);
        }
    }
}
