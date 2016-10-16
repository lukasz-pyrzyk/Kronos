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
            IExpiryProvider expiryProvider = Substitute.For<IExpiryProvider>();
            IStorage storage = new InMemoryStorage(expiryProvider); storage.AddOrUpdate(key, Encoding.UTF8.GetBytes(objectWord));

            byte[] objFromBytes = storage.TryGet(key);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.Equal(objectWord, stringFromBytes);
        }

        [Fact]
        public void CanUpdateExistingObject()
        {
            string key = "key";
            string first = "first";
            string second = "second";
            IExpiryProvider expiryProvider = Substitute.For<IExpiryProvider>();
            IStorage storage = new InMemoryStorage(expiryProvider);

            byte[] firstObject = Encoding.UTF8.GetBytes(first);
            byte[] secondObject = Encoding.UTF8.GetBytes(second);

            storage.AddOrUpdate(key, firstObject);
            storage.AddOrUpdate(key, secondObject);

            byte[] objFromBytes = storage.TryGet(key);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.Equal(stringFromBytes, second);
        }

        [Fact]
        public void TryRemove_RemovesEntryFromStorage()
        {
            byte[] package = Encoding.UTF8.GetBytes("lorem ipsum");
            const string firstKey = "key1";
            const string secondKey = "key2";

            IExpiryProvider expiryProvider = Substitute.For<IExpiryProvider>();
            IStorage storage = new InMemoryStorage(expiryProvider);
            storage.AddOrUpdate(firstKey, package);
            storage.AddOrUpdate(secondKey, package);

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

            IExpiryProvider expiryProvider = Substitute.For<IExpiryProvider>();
            IStorage storage = new InMemoryStorage(expiryProvider);
            storage.AddOrUpdate(firstKey, package);

            bool deleted = storage.TryRemove(secondKey);

            Assert.False(deleted);
            Assert.Equal(storage.Count, 1);
        }

        [Fact]
        public void CanClear()
        {
            IExpiryProvider expiryProvider = Substitute.For<IExpiryProvider>();
            IStorage storage = new InMemoryStorage(expiryProvider);

            storage.AddOrUpdate("first", new byte[0]);
            storage.AddOrUpdate("second", new byte[0]);

            storage.Dispose();
            Assert.Equal(storage.Count, 0);
        }

        [Fact]
        public void ReturnsNullWhenObjectDoesNotExist()
        {
            IExpiryProvider expiryProvider = Substitute.For<IExpiryProvider>();
            IStorage storage = new InMemoryStorage(expiryProvider);
            byte[] objFromBytes = storage.TryGet(string.Empty);

            Assert.Null(objFromBytes);
        }
    }
}
