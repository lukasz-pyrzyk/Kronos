using System;
using Google.Protobuf;
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
            const string key = "key";

            ByteString package = ByteString.CopyFromUtf8("lorem ipsum");
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            storage.AddOrUpdate(key, DateTime.MaxValue, package);

            ByteString objFromBytes;
            bool success = storage.TryGet(key, out objFromBytes);

            Assert.True(success);
            Assert.Equal(objFromBytes, package);
        }

        [Fact]
        public void Add_ReturnsTrue_WhenElementWasAdded()
        {
            // Arrange
            string key = "key";
            string objectWord = "lorem ipsum";
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            // Act
            bool added = storage.Add(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.Equal(storage.ExpiringCount, 1);
            Assert.True(added);
        }

        [Fact]
        public void AddOrUpdate_AddsElement()
        {
            // Arrange
            string key = "key";
            string objectWord = "lorem ipsum";
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            // Act
            storage.AddOrUpdate(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.Equal(storage.ExpiringCount, 1);
        }

        [Fact]
        public void Add_ReturnsFalse_WhenKeyAlreadyExists()
        {
            // Arrange
            string key = "key";
            string objectWord = "lorem ipsum";
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            // Act
            storage.Add(key, DateTime.MaxValue, ByteString.Empty);
            bool added = storage.Add(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.False(added);
        }

        [Fact]
        public void CanUpdateExistingObject()
        {
            string key = "key";
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            ByteString firstObject = ByteString.CopyFromUtf8("first");
            ByteString secondObject = ByteString.CopyFromUtf8("second");

            storage.AddOrUpdate(key, DateTime.MaxValue, firstObject);
            storage.AddOrUpdate(key, DateTime.MaxValue, secondObject);

            ByteString objFromBytes;
            bool success = storage.TryGet(key, out objFromBytes);

            Assert.True(success);
            Assert.Equal(objFromBytes, secondObject);
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage()
        {
            const string firstKey = "key1";
            const string secondKey = "key2";

            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);
            storage.AddOrUpdate(firstKey, DateTime.MaxValue, ByteString.Empty);
            storage.AddOrUpdate(secondKey, DateTime.MaxValue, ByteString.Empty);

            bool deleted = storage.TryRemove(firstKey);

            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void TryRemove_DoestNotRemoveEntryFromStorageWhenKeyDoesNotExist()
        {
            const string firstKey = "key1";
            const string secondKey = "key2";

            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);
            storage.AddOrUpdate(firstKey, DateTime.MaxValue, ByteString.Empty);

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
            storage.AddOrUpdate(key, DateTime.MaxValue, ByteString.Empty);
            storage.AddOrUpdate("second", DateTime.MaxValue, ByteString.Empty);

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

            storage.AddOrUpdate("first", DateTime.MaxValue, ByteString.Empty);
            storage.AddOrUpdate("second", DateTime.MaxValue, ByteString.Empty);

            storage.Dispose();
            Assert.Equal(storage.Count, 0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void Clear_ClearsTheData(int count)
        {
            var expiryProvider = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(expiryProvider);


            for (int i = 0; i < count; i++)
            {
                storage.AddOrUpdate(Guid.NewGuid().ToString(), DateTime.MaxValue, ByteString.Empty);
            }

            int deleted = storage.Clear();
            Assert.Equal(storage.Count, 0);
            Assert.Equal(deleted, count);
        }

        [Fact]
        public void ReturnsNullWhenObjectDoesNotExist()
        {
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = new InMemoryStorage(cleaner);

            ByteString objFromBytes;
            bool success = storage.TryGet("lorem ipsum", out objFromBytes);

            Assert.Null(objFromBytes);
            Assert.False(success);
        }
    }
}
