using System;
using System.IO;
using System.Text;
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
            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => new byte[0], s => { });

            Assert.Equal(storage.Count, 0);
        }

        [Fact]
        public void TryAdd_AddsObject()
        {
            string key = "lorem ipsum";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            bool called = false;

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { called = true; }, s => new byte[0], s => { });
            
            storage.AddOrUpdate(key, data);

            Assert.Equal(storage.Count, 1);
            Assert.True(called);
        }

        [Fact]
        public void TryAdd_DoesNotAddFileWhenExceptionWasThrown()
        {
            bool called = false;

            string key = "lorem ipsum";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { called = true; throw new IOException(); }, s => new byte[0], s => { });

            try
            {
                storage.AddOrUpdate(key, data);
            }
            catch (Exception)
            {
            }

            Assert.Equal(storage.Count, 0);
            Assert.True(called);
        }

        [Fact]
        public void TryGet_ReturnsObject()
        {
            string key = "lorem ipsum";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => data, s => { });

            storage.AddOrUpdate(key, data);

            byte[] obj = storage.TryGet(key);

            Assert.NotNull(obj);
            Assert.Equal(data, obj);
        }

        [Fact]
        public void TryGet_ReturnsNullWhenObjectIsNotAdded()
        {
            string key = "lorem ipsum";

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => new byte[0], s => { });

            byte[] obj = storage.TryGet(key);

            Assert.Null(obj);
        }

        [Fact]
        public void TryRemove_RemovesFile()
        {
            string key = "lorem ipsum";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => new byte[0], s => { });
            storage.AddOrUpdate(key, data);
            bool result = storage.TryRemove(key);

            Assert.True(result);
        }

        [Fact]
        public void TryRemove_DoesNotRemoveFileWhenObjectIsNotAdded()
        {
            string key = "lorem ipsum";

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => new byte[0], s => { });
            bool result = storage.TryRemove(key);

            Assert.False(result);
        }

        [Fact]
        public void TryRemove_DoesNotRemoveFileWhenRemoveMethodThrowsException()
        {
            string key = "lorem ipsum";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");

            bool called = false;

            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => new byte[0], s =>
            {
                called = true;
                throw new IOException();
            });

            storage.AddOrUpdate(key, data);
            bool result = storage.TryRemove(key);

            Assert.False(result);
            Assert.True(called);
        }

        [Fact]
        public void Clear_DeletesIndexFile()
        {
            bool called = false;
            DiscAndMemoryStorage storage = new DiscAndMemoryStorage((s, bytes) => { }, s => new byte[0], s =>
            {
                called = true;
            });

            storage.Clear();
            Assert.True(called);
        }
    }
}
