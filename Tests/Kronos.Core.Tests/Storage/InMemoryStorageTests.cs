using System;
using Google.Protobuf;
using Kronos.Core.Storage;
using Kronos.Core.Storage.Cleaning;
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
            IStorage storage = CreateStorage();

            storage.Add(key, DateTime.MaxValue, package);

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
            IStorage storage = CreateStorage();

            // Act
            bool added = storage.Add(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.Equal(storage.ExpiringCount, 1);
            Assert.True(added);
        }

        //[Fact]
        //public void AddOrUpdate_AddsElement()
        //{
        //    // Arrange
        //    string key = "key";
        //    string objectWord = "lorem ipsum";
        //    IStorage storage = CreateStorage();

        //    // Act
        //    storage.AddOrUpdate(key, DateTime.MaxValue, ByteString.Empty);

        //    // Assert
        //    Assert.Equal(storage.Count, 1);
        //    Assert.Equal(storage.ExpiringCount, 1);
        //}

        [Fact]
        public void Add_ReturnsFalse_WhenKeyAlreadyExists()
        {
            // Arrange
            string key = "key";
            string objectWord = "lorem ipsum";
            IStorage storage = CreateStorage();

            // Act
            storage.Add(key, DateTime.MaxValue, ByteString.Empty);
            bool added = storage.Add(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.False(added);
        }

        [Fact]
        public void ReturnsNullWhenObjectDoesNotExist()
        {
            IStorage storage = CreateStorage();

            ByteString objFromBytes;
            bool success = storage.TryGet("lorem ipsum", out objFromBytes);

            Assert.Null(objFromBytes);
            Assert.False(success);
        }

        //[Fact]
        //public void CanUpdateExistingObject()
        //{
        //    string key = "key";
        //    IStorage storage = CreateStorage();

        //    ByteString firstObject = ByteString.CopyFromUtf8("first");
        //    ByteString secondObject = ByteString.CopyFromUtf8("second");

        //    storage.Add(key, DateTime.MaxValue, firstObject);
        //    storage.Add(key, DateTime.MaxValue, secondObject); // add or update

        //    ByteString objFromBytes;
        //    bool success = storage.TryGet(key, out objFromBytes);

        //    Assert.True(success);
        //    Assert.Equal(objFromBytes, secondObject);
        //}

        [Fact]
        public void TryRemove_RemovesEntryFromStorage()
        {
            const string firstKey = "key1";
            const string secondKey = "key2";

            IStorage storage = CreateStorage();

            storage.Add(firstKey, DateTime.MaxValue, ByteString.Empty);
            storage.Add(secondKey, DateTime.MaxValue, ByteString.Empty);

            bool deleted = storage.TryRemove(firstKey);

            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void TryRemove_DoestNotRemoveEntryFromStorageWhenKeyDoesNotExist()
        {
            const string firstKey = "key1";
            const string secondKey = "key2";

            IStorage storage = CreateStorage();
            storage.Add(firstKey, DateTime.MaxValue, ByteString.Empty);

            bool deleted = storage.TryRemove(secondKey);

            Assert.False(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataExists()
        {
            // Arrange
            IStorage storage = CreateStorage();

            const string key = "lorem ipsum";
            storage.Add(key, DateTime.MaxValue, ByteString.Empty);
            storage.Add("second", DateTime.MaxValue, ByteString.Empty);

            // Act
            bool result = storage.Contains(key);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataDoesNotExist()
        {
            // Arrange
            IStorage storage = CreateStorage();

            // Assert
            bool result = storage.Contains("lorem ipsum");

            // Act
            Assert.False(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void Clear_ClearsTheData(int count)
        {
            // Arrange
            IStorage storage = CreateStorage();

            for (int i = 0; i < count; i++)
            {
                storage.Add(Guid.NewGuid().ToString(), DateTime.MaxValue, ByteString.Empty);
            }

            // Act
            int deleted = storage.Clear();

            // Assert
            Assert.Equal(storage.Count, 0);
            Assert.Equal(storage.ExpiringCount, 0);
            Assert.Equal(deleted, count);
        }

        [Fact]
        public void Dispose_ClearsTheData()
        {
            // Arrange
            IStorage storage = CreateStorage();
            
            storage.Add("first", DateTime.MaxValue, ByteString.Empty);
            storage.Add("second", DateTime.MaxValue, ByteString.Empty);

            // Act
            storage.Dispose();

            // Assert
            Assert.Equal(storage.Count, 0);
            Assert.Equal(storage.ExpiringCount, 0);
        }

        private static IStorage CreateStorage()
        {
            ICleaner cleaner = Substitute.For<ICleaner>();
            IScheduler scheduler = Substitute.For<IScheduler>();
            IStorage storage = new InMemoryStorage(cleaner, scheduler);
            return storage;
        }
    }
}
