using System;
using System.Text;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class InMemoryStorageTests
    {
        [Fact]
        public void CanInsertAndGetObject()
        {
            string key = "key";
            string objectWord = "lorem ipsum";
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner); storage.AddOrUpdate(key, DateTime.MaxValue, Encoding.UTF8.GetBytes(objectWord));

            byte[] objFromBytes;
            bool success = storage.TryGet(key, out objFromBytes);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.True(success);
            Assert.Equal(objectWord, stringFromBytes);
        }

        [Fact]
        public void CanUpdateExistingObject()
        {
            string key = "key";
            string first = "first";
            string second = "second";
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            byte[] firstObject = Encoding.UTF8.GetBytes(first);
            byte[] secondObject = Encoding.UTF8.GetBytes(second);

            storage.AddOrUpdate(key, DateTime.MaxValue, firstObject);
            storage.AddOrUpdate(key, DateTime.MaxValue, secondObject);

            byte[] objFromBytes;
            bool success = storage.TryGet(key, out objFromBytes);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.True(success);
            Assert.Equal(stringFromBytes, second);
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage()
        {
            byte[] package = Encoding.UTF8.GetBytes("lorem ipsum");
            const string firstKey = "key1";
            const string secondKey = "key2";

            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);
            storage.AddOrUpdate(firstKey, DateTime.MaxValue, package);
            storage.AddOrUpdate(secondKey, DateTime.MaxValue, package);

            bool deleted = storage.TryRemove(firstKey);

            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void TryRemove_DoestNotRemoveEntryFromStorageWhenKeyDoesNotExist()
        {
            byte[] package = Encoding.UTF8.GetBytes("lorem ipsum");
            const string firstKey = "key1";
            const string secondKey = "key2";

            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);
            storage.AddOrUpdate(firstKey, DateTime.MaxValue, package);

            bool deleted = storage.TryRemove(secondKey);

            Assert.False(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataExists()
        {
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            string key = "lorem ipsum";
            storage.AddOrUpdate(key, DateTime.MaxValue, new byte[0]);
            storage.AddOrUpdate("second", DateTime.MaxValue, new byte[0]);

            bool result = storage.Contains(key);

            Assert.True(result);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataDoesNotExist()
        {
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            string key = "lorem ipsum";

            bool result = storage.Contains(key);

            Assert.False(result);
        }

        [Fact]
        public void CanClear()
        {
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            storage.AddOrUpdate("first", DateTime.MaxValue, new byte[0]);
            storage.AddOrUpdate("second", DateTime.MaxValue, new byte[0]);

            storage.Dispose();
            Assert.Equal(storage.Count, 0);
        }

        [Fact]
        public void Clear_ClearsTheData()
        {
            var expiryProvider = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(expiryProvider);

            storage.AddOrUpdate("first", DateTime.MaxValue, new byte[0]);
            storage.AddOrUpdate("second", DateTime.MaxValue, new byte[0]);

            storage.Clear();
            Assert.Equal(storage.Count, 0);
        }

        [Fact]
        public void ReturnsNullWhenObjectDoesNotExist()
        {
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            byte[] objFromBytes;
            bool success = storage.TryGet("lorem ipsum", out objFromBytes);

            Assert.Null(objFromBytes);
            Assert.False(success);
        }
    }
}
