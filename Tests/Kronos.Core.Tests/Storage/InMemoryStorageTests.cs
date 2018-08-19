using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Core.Pooling;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using Kronos.Core.Storage.Cleaning;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class InMemoryStorageTests
    {
        private ReadOnlyMemory<byte> validKey = "key".GetMemory();
        private ReadOnlyMemory<byte> invalidKey = "lorem ipsum".GetMemory();

        [Fact]
        public void Add_ReturnsTrue_WhenElementWasAdded()
        {
            // Arrange
            IStorage storage = CreateStorage();

            // Act
            bool added = storage.Add(validKey, null, new byte[0]);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.True(added);
        }

        [Fact]
        public void Add_ReturnsTrue_WhenElementWasAdded_AndToTheExpiringKeys()
        {
            // Arrange

            IStorage storage = CreateStorage();

            // Act
            bool added = storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);

            // Assert
            Assert.Equal(storage.Count, 1);
            Assert.Equal(storage.ExpiringCount, 1);
            Assert.True(added);
        }

        [Fact]
        public void Add_ReturnsFalse_WhenKeyAlreadyExists()
        {
            // Arrange
            IStorage storage = CreateStorage();

            // Act
            storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);
            bool added = storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);

            // Assert
            Assert.False(added);
        }

        [Fact]
        public async Task Add_OverridesValue_WhenKeyWasExpired()
        {
            // Arrange
            IStorage storage = CreateStorage();
            TimeSpan expiryTime = TimeSpan.FromSeconds(1);
            var now = DateTimeOffset.UtcNow;

            // Act
            storage.Add(validKey, now + expiryTime, new byte[0]);
            await Task.Delay(TimeSpan.FromTicks(expiryTime.Ticks * 2)); // multiply by 2, mono behaves differently... Race condition?

            bool added = storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);

            // Assert
            Assert.True(added);
        }

        [Fact]
        public void TryGet_ReturnsObject()
        {
            // Arrange
            IStorage storage = CreateStorage();
            var data = "lorem ipsum".GetMemory();
            storage.Add(validKey, null, data);

            // Act
            bool success = storage.TryGet(validKey, out var received);

            // Assert
            success.Should().BeTrue();
            received.Span.SequenceEqual(data.Span).Should().BeTrue();
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage()
        {
            // Arrange
            IStorage storage = CreateStorage();

            storage.Add(validKey, null, new byte[0]);
            storage.Add(invalidKey, null, new byte[0]);

            // Act
            bool deleted = storage.TryRemove(validKey);

            // Assert
            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage_AlsoFromExpiringKeys()
        {
            // Arrange
            IStorage storage = CreateStorage();

            storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);
            storage.Add(invalidKey, DateTimeOffset.MaxValue, new byte[0]);

            // Act
            bool deleted = storage.TryRemove(validKey);

            // Assert
            Assert.True(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void TryRemove_DoestNotRemoveEntryFromStorageWhenKeyDoesNotExist()
        {
            IStorage storage = CreateStorage();
            storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);

            bool deleted = storage.TryRemove(invalidKey);

            Assert.False(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataExists()
        {
            // Arrange
            IStorage storage = CreateStorage();
            storage.Add(validKey, null, new byte[0]);

            // Act
            bool result = storage.Contains(validKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_ReturnsFalseWhenKeyIsExpired()
        {
            // Arrange
            IStorage storage = CreateStorage();
            storage.Add(validKey, DateTimeOffset.MinValue, new byte[0]);

            // Act
            bool result = storage.Contains(validKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataDoesNotExist()
        {
            // Arrange
            IStorage storage = CreateStorage();

            // Assert
            bool result = storage.Contains(validKey);

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
                storage.Add(Guid.NewGuid().ToString().GetMemory(), DateTimeOffset.MaxValue, new byte[0]);
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

            storage.Add(validKey, DateTimeOffset.MaxValue, new byte[0]);
            storage.Add(invalidKey, DateTimeOffset.MaxValue, new byte[0]);

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
            storage.Add(invalidKey, null, new byte[0]);

            // Assert
            cleaner.Received(1).Clear(Arg.Any<PriorityQueue<ExpiringKey>>(), Arg.Any<IStorage>());
        }

        [Fact]
        public async Task TryGet_CallsCleaner()
        {
            // Arrange
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = await CreateStorageWithSchedulerAndWait(cleaner);

            // Act
            storage.TryGet(invalidKey, out var _);

            // Assert
            cleaner.Received(1).Clear(Arg.Any<PriorityQueue<ExpiringKey>>(), Arg.Any<IStorage>());
        }

        [Fact]
        public async Task Contains_CallsCleaner()
        {
            // Arrange
            ICleaner cleaner = Substitute.For<ICleaner>();
            IStorage storage = await CreateStorageWithSchedulerAndWait(cleaner);

            // Act
            storage.Contains(invalidKey);

            // Assert
            cleaner.Received(1).Clear(Arg.Any<PriorityQueue<ExpiringKey>>(), Arg.Any<IStorage>());
        }

        private static async Task<IStorage> CreateStorageWithSchedulerAndWait(ICleaner cleaner)
        {
            const int timePeriod = 10;
            IScheduler scheduler = new Scheduler();
            InMemoryStorage storage = new InMemoryStorage(cleaner, scheduler, new ServerMemoryPool());

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
            IStorage storage = new InMemoryStorage(cleaner, scheduler, new ServerMemoryPool());
            return storage;
        }
    }
}
