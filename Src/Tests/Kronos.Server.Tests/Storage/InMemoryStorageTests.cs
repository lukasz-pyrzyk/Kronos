using System.Text;
using Kronos.Server.Storage;
using Xunit;

namespace Kronos.Server.Tests.Storage
{
    public class InMemoryStorageTests
    {
        [Fact]
        public void CanInsertAndGetObject()
        {
            string key = "key";
            string objectWord = "lorem ipsum";
            InMemoryStorage.AddOrUpdate(key, Encoding.UTF8.GetBytes(objectWord));

            byte[] objFromBytes = InMemoryStorage.TryGet(key);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.Equal(objectWord, stringFromBytes);

            InMemoryStorage.Clear();
        }

        [Fact]
        public void CanUpdateExistingObject()
        {
            string key = "key";
            string first = "first";
            string second = "second";

            byte[] firstObject = Encoding.UTF8.GetBytes(first);
            byte[] secondObject = Encoding.UTF8.GetBytes(second);

            InMemoryStorage.AddOrUpdate(key, firstObject);
            InMemoryStorage.AddOrUpdate(key, secondObject);

            byte[] objFromBytes = InMemoryStorage.TryGet(key);
            string stringFromBytes = Encoding.UTF8.GetString(objFromBytes);

            Assert.Equal(stringFromBytes, second);

            InMemoryStorage.Clear();
        }

        [Fact]
        public void CanClear()
        {
            InMemoryStorage.AddOrUpdate("first", new byte[0]);
            InMemoryStorage.AddOrUpdate("second", new byte[0]);

            InMemoryStorage.Clear();

            Assert.Equal(InMemoryStorage.Count, 0);
        }

        [Fact]
        public void ReturnsNullWhenObjectDoesNotExist()
        {
            byte[] objFromBytes = InMemoryStorage.TryGet(string.Empty);

            Assert.Null(objFromBytes);

            InMemoryStorage.Clear();
        }
    }
}
