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
        public void AddOrUpdate_AddsFileToIndexDictionaryAndWritesToFiles()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[] { });
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[] { });

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
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[] { });
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[] { });

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            storage.Clear();

            storageMock.Verify(x => x.Dispose(), Times.Once);
            indexMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_DisposesStreams()
        {
            var storageMock = new Mock<IFileStream>();
            storageMock.Setup(x => x.EnumerateLines()).Returns(new string[] { });
            var indexMock = new Mock<IFileStream>();
            indexMock.Setup(x => x.EnumerateLines()).Returns(new string[] { });

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage(p => indexMock.Object, p => storageMock.Object, path => { });

            storage.Dispose();

            storageMock.Verify(x => x.Dispose(), Times.Once);
            indexMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
