using System.Text;
using Kronos.Core.Storage;
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
            IStorage storage = new InMemoryStorage();
            storage.AddOrUpdate(key, Encoding.UTF8.GetBytes(objectWord));

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
            IStorage storage = new InMemoryStorage();

            byte[] firstObject = Encoding.UTF8.GetBytes(first);
            byte[] secondObject = Encoding.UTF8.GetBytes(second);

            storage.AddOrUpdate(key, firstObject);
            storage.AddOrUpdate(key, secondObject);

            byte[] objFromBytes = storage.TryGet(key);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.Equal(stringFromBytes, second);
        }

        [Fact]
        public void CanClear()
        {
            IStorage storage = new InMemoryStorage();

            storage.AddOrUpdate("first", new byte[0]);
            storage.AddOrUpdate("second", new byte[0]);

            storage.Clear();

            Assert.Equal(storage.Count, 0);
        }

        [Fact]
        public void ReturnsNullWhenObjectDoesNotExist()
        {
            IStorage storage = new InMemoryStorage();
            byte[] objFromBytes = storage.TryGet(string.Empty);

            Assert.Null(objFromBytes);
        }
    }
}
