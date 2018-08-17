using System;
using System.Collections.Generic;
using System.Text;
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
            bool added = storage.Add(key, null, new byte[0]);

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
            bool added = storage.Add(key, DateTimeOffset.MaxValue, new byte[0]);

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
            storage.Add(key, DateTimeOffset.MaxValue, new byte[0]);
            bool added = storage.Add(key, DateTimeOffset.MaxValue, new byte[0]);

            // Assert
            Assert.False(added);
        }

        [Fact]
        public async Task Add_OverridesValue_WhenKeyWasExpired()
        {
            // Arrange
            const string key = "key";
            IStorage storage = CreateStorage();
            TimeSpan expiryTime = TimeSpan.FromSeconds(1);
            var now = DateTimeOffset.UtcNow;

            // Act
            storage.Add(key, now + expiryTime, new byte[0]);
            await Task.Delay(TimeSpan.FromTicks(expiryTime.Ticks * 2)); // multiply by 2, mono behaves differently... Race condition?

            bool added = storage.Add(key, DateTimeOffset.MaxValue, new byte[0]);

            // Assert
            Assert.True(added);
        }

        [Fact]
        public void TryGet_ReturnsObject()
        {
            // Arrange
            IStorage storage = CreateStorage();
            const string key = "lorem ipsum";
            var data = Encoding.UTF8.GetBytes("lorem ipsum");
            storage.Add(key, null, data);

            // Act
            bool success = storage.TryGet(key, out var received);

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
            bool success = storage.TryGet("lorem ipsum", out var received);

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
            var data = Encoding.UTF8.GetBytes("lorem ipsum");
            storage.Add(key, DateTimeOffset.MinValue, data);

            // Act
            bool success = storage.TryGet(key, out var received);

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

            storage.Add(firstKey, null, new byte[0]);
            storage.Add(secondKey, null, new byte[0]);

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

            storage.Add(firstKey, DateTimeOffset.MaxValue, new byte[0]);
            storage.Add(secondKey, DateTimeOffset.MaxValue, new byte[0]);

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
            storage.Add(firstKey, DateTimeOffset.MaxValue, new byte[0]);

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
            storage.Add(key, null, new byte[0]);

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
            storage.Add(key, DateTimeOffset.MinValue, new byte[0]);

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
                storage.Add(Guid.NewGuid().ToString(), DateTimeOffset.MaxValue, new byte[0]);
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

            storage.Add("first", DateTimeOffset.MaxValue, new byte[0]);
            storage.Add("second", DateTimeOffset.MaxValue, new byte[0]);

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
            storage.Add("", null, new byte[0]);

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
            storage.TryGet("", out var _);

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
            IScheduler scheduler = new Scheduler();
            InMemoryStorage storage = new InMemoryStorage(cleaner, scheduler);

            do
            {
                await Task.Delay(timePeriod);
            } while (!storage.CleanupRequested);

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
