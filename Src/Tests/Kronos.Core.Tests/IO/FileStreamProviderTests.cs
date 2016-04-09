using System;
using System.IO;
using Kronos.Core.IO;
using Xunit;

namespace Kronos.Core.Tests.IO
{
    public class FileStreamProviderTests
    {
        [Fact]
        public void Ctor_AsingsStream()
        {
            Stream stream = Stream.Null;
            FileStreamProvider provider = new FileStreamProvider(stream);

            Assert.NotNull(provider);
            Assert.Equal(provider.Stream, stream);
            Assert.Equal(provider.Length, stream.Length);
            Assert.Equal(provider.Position, stream.Position);
        }

        [Fact]
        public void Open_OpensFileStream()
        {
            string path = Guid.NewGuid().ToString();

            IFileStream stream = FileStreamProvider.Open(path);
            stream.Dispose();

            Assert.NotNull(stream);
            File.Delete(path);
        }
    }
}
