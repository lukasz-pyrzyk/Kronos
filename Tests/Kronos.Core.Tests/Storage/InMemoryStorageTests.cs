using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public void Add_ReturnsTrue_WhenElementWasAdded()
        {
            // Arrange
            const string key = "key";
            IStorage storage = CreateStorage();

            // Act
            bool added = storage.Add(key, null, ByteString.Empty);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.True(added);
        }

        [Fact]
        public void Add_ReturnsTrue_WhenElementWasAdded_AndToTheExpiringKeys()
        {
            // Arrange
            const string key = "key";
            IStorage storage = CreateStorage();

            // Act
            bool added = storage.Add(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.Equal(storage.ExpiringCount, 1);
            Assert.True(added);
        }

        [Fact]
        public void Add_ReturnsFalse_WhenKeyAlreadyExists()
        {
            // Arrange
            const string key = "key";
            IStorage storage = CreateStorage();

            // Act
            storage.Add(key, DateTime.MaxValue, ByteString.Empty);
            bool added = storage.Add(key, DateTime.MaxValue, ByteString.Empty);

            // Assert
            Assert.False(added);
        }

        [Fact]
        public void TryGet_ReturnsObject()
        {
            // Arrange
            IStorage storage = CreateStorage();
            const string key = "lorem ipsum";
            ByteString data = ByteString.CopyFromUtf8("lorem ipsum");
            storage.Add(key, null, data);

            // Act
            ByteString received;
            bool success = storage.TryGet(key, out received);

            // Assert
            Assert.True(success);
            Assert.Equal(data, received);
        }

        [Fact]
        public void TryGet_ReturnsNullWhenObjectDoesNotExist()
        {
            // Arrange
            IStorage storage = CreateStorage();

            // Act
            ByteString received;
            bool success = storage.TryGet("lorem ipsum", out received);

            // Assert
            Assert.False(success);
            Assert.Null(received);
        }

        [Fact]
        public void TryGet_ReturnsNullWhenObjectIsExpired()
        {
            // Arrange
            IStorage storage = CreateStorage();
            const string key = "lorem ipsum";
            ByteString data = ByteString.CopyFromUtf8("lorem ipsum");
            storage.Add(key, DateTime.MinValue, data);

            // Act
            ByteString received;
            bool success = storage.TryGet(key, out received);

            // Assert
            Assert.False(success);
            Assert.Null(received);
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage()
        {
            // Arrange
            const string firstKey = "key1";
            const string secondKey = "key2";

            IStorage storage = CreateStorage();

            storage.Add(firstKey, null, ByteString.Empty);
            storage.Add(secondKey, null, ByteString.Empty);

            // Act
            bool deleted = storage.TryRemove(firstKey);

            // Assert
            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage_AlsoFromExpiringKeys()
        {
            // Arrange
            const string firstKey = "key1";
            const string secondKey = "key2";

            IStorage storage = CreateStorage();

            storage.Add(firstKey, DateTime.MaxValue, ByteString.Empty);
            storage.Add(secondKey, DateTime.MaxValue, ByteString.Empty);

            // Act
            bool deleted = storage.TryRemove(firstKey);

            // Assert
            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
            Assert.Equal(storage.ExpiringCount, 1);
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
            storage.Add(key, null, ByteString.Empty);

            // Act
            bool result = storage.Contains(key);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_ReturnsFalseWhenKeyIsExpired()
        {
            // Arrange
            IStorage storage = CreateStorage();
            const string key = "lorem ipsum";
            storage.Add(key, DateTime.MinValue, ByteString.Empty);

            // Act
            bool result = storage.Contains(key);

            // Assert
            Assert.False(result);
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

        [Fact]
        public async Task Add_CallsCleaner()
        {
            // Arrange
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = await CreateStorageWithSchedulerAndWait(cleaner);

            // Act
            storage.Add("", null, ByteString.Empty);

            // Assert
            cleaner.Received(1).Clear(Arg.Any<PriorityQueue<ExpiringKey>>(), Arg.Any<Dictionary<Key, Element>>());
        }

        [Fact]
        public async Task TryGet_CallsCleaner()
        {
            // Arrange
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = await CreateStorageWithSchedulerAndWait(cleaner);

            // Act
            ByteString elem;
            storage.TryGet("", out elem);

            // Assert
            cleaner.Received(1).Clear(Arg.Any<PriorityQueue<ExpiringKey>>(), Arg.Any<Dictionary<Key, Element>>());
        }

        [Fact]
        public async Task Contains_CallsCleaner()
        {
            // Arrange
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = await CreateStorageWithSchedulerAndWait(cleaner);

            // Act
            storage.Contains("");

            // Assert
            cleaner.Received(1).Clear(Arg.Any<PriorityQueue<ExpiringKey>>(), Arg.Any<Dictionary<Key, Element>>());
        }

        private static async Task<IStorage> CreateStorageWithSchedulerAndWait(ICleaner cleaner)
        {
            const int timePeriod = 10;
            IScheduler scheduler = new Scheduler(timePeriod);
            InMemoryStorage storage = new InMemoryStorage(cleaner, scheduler);

            do
            {
                await Task.Delay(timePeriod);
            } while (storage.cleanupRequested != 1);

            return storage;
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
