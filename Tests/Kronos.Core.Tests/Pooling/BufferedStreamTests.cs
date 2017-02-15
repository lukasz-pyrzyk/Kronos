using System;
using System.IO;
using System.Linq;
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
            var stream = new BufferedStream();

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
            var stream = new BufferedStream();

            // Act 
            bool isClean = stream.IsClean;

            // Assert
            Assert.True(isClean);
        }

        [Fact]
        public void IsClean_IsFalse_WhenDataIsWritten()
        {
            // Arrange
            var stream = new BufferedStream();
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
            var stream = new BufferedStream();
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
            var stream = new BufferedStream();

            // Act 
            bool value = stream.CanRead;

            // Assert
            Assert.True(value);
        }

        [Fact]
        public void CanWrite_IsTrue()
        {
            // Arrange
            var stream = new BufferedStream();

            // Act 
            bool value = stream.CanWrite;

            // Assert
            Assert.True(value);
        }

        [Fact]
        public void CanSeek_IsFalse()
        {
            // Arrange
            var stream = new BufferedStream();

            // Act 
            bool value = stream.CanSeek;

            // Assert
            Assert.False(value);
        }

        [Fact]
        public void Length_IsMovedByReservedBytesForSize()
        {
            // Arrange
            var stream = new BufferedStream();
            
            // Act and assert
            Assert.Equal(stream.Position, sizeof(int));
        }

        [Fact]
        public void Position_IsMovedAfterWriting()
        {
            // Arrange
            var stream = new BufferedStream();
            
            // Act and assert
            Assert.Equal(stream.Position, sizeof(int));
        }

        [Fact]
        public void Length_IsMovedAfterWriting()
        {
            // Arrange
            var stream = new BufferedStream();
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
            var stream = new BufferedStream();
            int length = (int)stream.Length;
            byte[] data = new byte[1024];

            // Act
            stream.Write(data, 0, data.Length);

            // Assert
            Assert.Equal(stream.Length, length + data.Length);
        }

        [Fact]
        public void Flush_DoesNotThrow()
        {
            // Arrange
            var stream = new BufferedStream();

            // Act 
            var ex = Record.Exception(() => stream.Flush());

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void Clean_ResetsPositions()
        {
            // Arrange
            var stream = new BufferedStream();
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
            var stream = new BufferedStream();

            // Act 
            var ex = Record.Exception(() => stream.SetLength(0));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Seek_Begin_ChangesPosition()
        {
            // Arrange
            var stream = new BufferedStream();
            long initPosition = stream.Position;
            const int toMove = 2;
            var data = new byte[1024];
            
            // Act
            stream.Write(data, 0, data.Length);
            stream.Seek(toMove, SeekOrigin.Begin);

            // Act and assert
            Assert.Equal(stream.Position, initPosition + toMove);
        }

        [Fact]
        public void Seek_End_ChangesPosition()
        {
            // Arrange
            var stream = new BufferedStream();
            const int toMove = 2;
            var data = new byte[1024];
            
            // Act
            stream.Write(data, 0, data.Length);
            long position = stream.Position;
            stream.Seek(toMove, SeekOrigin.End);

            // Act and assert
            Assert.Equal(stream.Position, position - toMove);
        }
        
        [Fact]
        public void Seek_Current_ChangesPosition()
        {
            // Arrange
            var stream = new BufferedStream();
            var data = new byte[1024];
            long toMove = -2;

            // Act
            stream.Write(data, 0, data.Length);
            long position = stream.Position;
            stream.Seek(toMove, SeekOrigin.Current);

            // Act and assert
            Assert.Equal(stream.Position, position + toMove);
        }

        [Fact]
        public void Seek_ThrowsEndOfStream_WhenValueIsToBig()
        {
            // Arrange
            var stream = new BufferedStream();
            
            // Act and assert
            Assert.Throws<EndOfStreamException>(() => stream.Seek(int.MaxValue, SeekOrigin.Begin));
        }

        [Fact]
        public void Read_ReadsDataFromStream_WhenCountIsTheSame()
        {
            // Arrange
            var stream = new BufferedStream();
            byte[] data = new byte[1024];
            new Random().NextBytes(data);

            // Act 
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] receivedBytes = new byte[data.Length];
            stream.Read(receivedBytes, 0, receivedBytes.Length);

            // Assert
            Assert.Equal(data, receivedBytes);
        }

        [Fact]
        public void Read_ReadsDataFromStream_WhenCountIsBigger()
        {
            // Arrange
            var stream = new BufferedStream();
            byte[] data = new byte[1024];
            new Random().NextBytes(data);

            // Act 
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] receivedBytes = new byte[data.Length + 1];

            // Assert
            Assert.Throws<EndOfStreamException>(() => stream.Read(receivedBytes, 0, receivedBytes.Length));
        }

        [Fact]
        public void Read_ReadsDataFromStream_WhenCountIsSmaller()
        {
            // Arrange
            var stream = new BufferedStream();
            byte[] data = new byte[1024];
            new Random().NextBytes(data);

            // Act 
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] receivedBytes = new byte[data.Length - 1];
            stream.Read(receivedBytes, 0, receivedBytes.Length);

            // Assert
            Assert.Equal(data.Take(receivedBytes.Length), receivedBytes);
        }
    }
}
