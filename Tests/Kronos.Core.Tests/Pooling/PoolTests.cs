using Kronos.Core.Pooling;
using Xunit;

namespace Kronos.Core.Tests.Pooling
{
    public abstract class PoolTests
    {
        protected abstract Pool<byte> Create(int count = 100);

        [Fact]
        public void Ctor_PreallocatesPool()
        {
            // Arrange and act
            var pool = Create();

            // Assert
            Assert.True(pool.Count > 0);
        }

        [Fact]
        public void Ctor_PreallocatesPoolWithSpecialSize()
        {
            // Arrange
            const int size = 50;

            // Act
            var pool = Create(size);

            // Assert
            Assert.Equal(pool.Count, size);
        }

        [Fact]
        public void Rent_ReturnsOneElement()
        {
            // Arrange
            var pool = Create();
            int count = pool.Count;

            // Act
            pool.Rent();

            // Assert
            Assert.Equal(pool.Count, count - 1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(999999)]
        public void Rent_ReturnsNElement(int toRent)
        {
            // Arrange
            var pool = Create();

            // Act
            byte[] elements = pool.Rent(toRent);

            // Assert
            Assert.True(pool.Count > 0, "Pool is empty");
            Assert.Equal(elements.Length, toRent);
        }

        [Fact]
        public void Return_ReturnsElement()
        {
            // Arrange
            var pool = Create();
            int count = pool.Count;
            var element = new byte();

            // Act
            pool.Return(element);

            // Assert
            Assert.Equal(pool.Count, count + 1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(99999)]
        public void Return_ReturnsNElements(int toReturn)
        {
            // Arrange
            var pool = Create();
            int count = pool.Count;
            var elements = new byte[toReturn];

            // Act
            pool.Return(elements);

            // Assert
            Assert.Equal(pool.Count, count + toReturn);
        }

        [Fact]
        public void Use_ReturnsElement()
        {
            // Arrange
            var pool = Create();
            int count = pool.Count;

            // Act
            pool.Use(client =>
            {
            });

            // Assert
            Assert.Equal(pool.Count, count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void Use_ReturnsNElements(int toReturn)
        {
            // Arrange
            var pool = Create();
            int count = pool.Count;

            // Act
            pool.Use(toReturn, client =>
            {
            });

            // Assert
            Assert.Equal(pool.Count, count);
        }
    }
}
