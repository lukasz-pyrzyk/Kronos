using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Server.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Storage
{
    public class InMemoryStorageTests
    {
        [Fact]
        public void Add_ReturnsTrue_WhenElementWasAdded()
        {
            // Arrange
            const string key = "key";
            var storage = CreateStorage();

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
            var storage = CreateStorage();

            // Act
            bool added = storage.Add(key, DateTimeOffset.MaxValue, ByteString.Empty);

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
            var storage = CreateStorage();

            // Act
            storage.Add(key, DateTimeOffset.MaxValue, ByteString.Empty);
            bool added = storage.Add(key, DateTimeOffset.MaxValue, ByteString.Empty);

            // Assert
            Assert.False(added);
        }

        [Fact]
        public async Task Add_OverridesValue_WhenKeyWasExpired()
        {
            // Arrange
            const string key = "key";
            var storage = CreateStorage();
            TimeSpan expiryTime = TimeSpan.FromSeconds(1);
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Act
            storage.Add(key, now + expiryTime, ByteString.Empty);
            await Task.Delay(TimeSpan.FromTicks(expiryTime.Ticks * 2)); // multiply by 2, mono behaves differently... Race condition?

            bool added = storage.Add(key, DateTimeOffset.MaxValue, ByteString.Empty);

            // Assert
            Assert.True(added);
        }

        [Fact]
        public void TryGet_ReturnsObject()
        {
            // Arrange
            var storage = CreateStorage();
            const string key = "lorem ipsum";
            ByteString data = ByteString.CopyFromUtf8("lorem ipsum");
            storage.Add(key, null, data);

            // Act
            bool success = storage.TryGet(key, out ByteString received);

            // Assert
            Assert.True(success);
            Assert.Equal(data, received);
        }

        [Fact]
        public void TryGet_ReturnsNullWhenObjectDoesNotExist()
        {
            // Arrange
            var storage = CreateStorage();

            // Act
            bool success = storage.TryGet("lorem ipsum", out ByteString received);

            // Assert
            Assert.False(success);
            Assert.Null(received);
        }

        [Fact]
        public void TryGet_ReturnsNullWhenObjectIsExpired()
        {
            // Arrange
            var storage = CreateStorage();
            const string key = "lorem ipsum";
            ByteString data = ByteString.CopyFromUtf8("lorem ipsum");
            storage.Add(key, DateTimeOffset.MinValue, data);

            // Act
            bool success = storage.TryGet(key, out ByteString received);

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

            var storage = CreateStorage();

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

            var storage = CreateStorage();

            storage.Add(firstKey, DateTimeOffset.MaxValue, ByteString.Empty);
            storage.Add(secondKey, DateTimeOffset.MaxValue, ByteString.Empty);

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

            var storage = CreateStorage();
            storage.Add(firstKey, DateTimeOffset.MaxValue, ByteString.Empty);

            bool deleted = storage.TryRemove(secondKey);

            Assert.False(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataExists()
        {
            // Arrange
            var storage = CreateStorage();
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
            var storage = CreateStorage();
            const string key = "lorem ipsum";
            storage.Add(key, DateTimeOffset.MinValue, ByteString.Empty);

            // Act
            bool result = storage.Contains(key);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenDataDoesNotExist()
        {
            // Arrange
            var storage = CreateStorage();

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
            var storage = CreateStorage();

            for (int i = 0; i < count; i++)
            {
                storage.Add(Guid.NewGuid().ToString(), DateTimeOffset.MaxValue, ByteString.Empty);
            }

            // Act
            int deleted = storage.Clear();

            // Assert
            Assert.Equal(storage.Count, 0);
            Assert.Equal(storage.ExpiringCount, 0);
            Assert.Equal(deleted, count);
        }

        private static InMemoryStorage CreateStorage()
        {
            ILogger<InMemoryStorage> logger = Substitute.For<ILogger<InMemoryStorage>>();
            return new InMemoryStorage(logger);
        }
    }
}
