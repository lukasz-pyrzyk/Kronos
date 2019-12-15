using System;
using System.Collections.Generic;
using System.Linq;
using Kronos.Server.Storage;
using Xunit;

namespace Kronos.Server.Tests.Storage
{
    public class ConcurrentPriorityQueueTests
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Peek_ReturnsElement(int[] values, int[] expectedOrder)
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            foreach (int value in values)
            {
                queue.Add(value);
            }

            // Act
            int[] received = queue.ToArray();

            // Assert
            Assert.Equal(expectedOrder, received);

        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[] { new[] { 1, 2, 3 }, new[] { 3, 2, 1 } };
            yield return new object[] { new[] { 3, 2, 1 }, new[] { 3, 2, 1 } };
            yield return new object[] { new[] { 3, 3, 1 }, new[] { 3, 3, 1 } };
            yield return new object[] { new[] { 3, 3, 3 }, new[] { 3, 3, 3 } };
        }

        [Fact]
        public void Peek_ThrowsExceptionWhenElementIsMissing()
        {
            // Arrange
            var queue = new PriorityQueue<int>();

            // Act and assert
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        [Fact]
        public void Poll_ThrowsExceptionWhenElementIsMissing()
        {
            // Arrange
            var queue = new PriorityQueue<int>();

            // Act and assert
            Assert.Throws<InvalidOperationException>(() => queue.Poll());
        }

        [Fact]
        public void Poll_ReturnsElement()
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            const int element = 1;
            queue.Add(element);

            // Act
            int received = queue.Poll();

            // Assert
            Assert.Equal(received, element);
            Assert.Equal(queue.Count, 0);
        }

        [Fact]
        public void Remove_RemovesElement()
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            const int element = 1;
            queue.Add(element);

            // Act
            queue.Remove(element);

            // Assert
            Assert.Equal(queue.Count, 0);
        }

        [Fact]
        public void Clear_ClearsElements()
        {
            // Arrange
            var queue = new PriorityQueue<int>();
            queue.Add(1);

            // Act
            queue.Clear();

            // Assert
            Assert.Equal(queue.Count, 0);
        }
    }
}
