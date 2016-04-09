using System;
using System.IO;
using System.Text;
using Kronos.Core.IO;
using Kronos.Core.Storage;
using Moq;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class DiscAndMemoryStorageTests
    {
        [Fact]
        public void Ctor_InitializeEmptyIndexCollectionWhenFileIsEmpty()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            Assert.Equal(storage.Count, 0);
        }

        [Fact]
        public void Ctor_LoadsIndexesFromFile()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new[] { $"key;0;0;{Environment.NewLine}" });

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });
            
            Assert.Equal(storage.Count, 1);
        }
        
        [Fact]
        public void TryGet_ReturnsFileFromStorage()
        {
            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("lorem ipsum");

            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);
            storageMock.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), package.Length))
                .Callback<byte[], int, int>(
                    (bytes, offset, length) =>
                    {
                        for (int i = 0; i < length; i++)
                        {
                            bytes[i] = package[i];
                        }
                    });

            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new[] { $"{key};{package.Length};0;{Environment.NewLine}" });

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            byte[] bytesFromStorage = storage.TryGet(key);

            Assert.NotNull(bytesFromStorage);
            Assert.Equal(bytesFromStorage, package);

            storageMock.Verify(x => x.Seek(0, It.IsAny<SeekOrigin>()), Times.AtLeastOnce);
            storageMock.Verify(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void TryGet_ReturnsNullWhenKeyDoesNotExist()
        {
            string key = "key";

            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            byte[] bytesFromStorage = storage.TryGet(key);

            Assert.Null(bytesFromStorage);

            storageMock.Verify(x => x.Seek(It.IsAny<int>(), It.IsAny<SeekOrigin>()), Times.Never);
            storageMock.Verify(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void AddOrUpdate_AddsFileToIndexDictionaryAndWritesToFiles()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            byte[] buffer = Encoding.UTF8.GetBytes("lorem ipsum");
            string key = "key";

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            int count = storage.Count;

            storage.AddOrUpdate(key, buffer);

            storageMock.Verify(x => x.Write(buffer, 0, buffer.Length), Times.Once);
            storageMock.Verify(x => x.Flush(), Times.Once);

            indexMock.Verify(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            indexMock.Verify(x => x.Flush(), Times.Once);

            Assert.Equal(storage.Count, count + 1);
        }



        [Fact]
        public void Clear_DisposesAndDeletesStreams()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            storage.Clear();

            storageMock.Verify(x => x.Dispose(), Times.Once);
            indexMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_DisposesStreams()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[0]);

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            storage.Dispose();

            storageMock.Verify(x => x.Dispose(), Times.Once);
            indexMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
