using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Core.Hashing;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.Hashing
{
    public class HasherTests
    {
        const string word = "lorem ipsum";

        [Fact]
        public void WorksWithMemory()
        {
            // Arrange
            var normalHash = Hasher.Hash(word);
            var span = word.GetMemory().Span;

            // Act
            var hashFromBytes = Hasher.Hash(span);

            // Assert
            hashFromBytes.Should().Be(normalHash);
        }

        [Fact]
        public void HashSecure()
        {
            // Arrange
            byte[] expectedBytes = Hasher.SecureHash(word);

            // Act
            for (int i = 0; i < 10000; i++)
            {
                byte[] bytes = Hasher.SecureHash(word);

                Assert.Equal(expectedBytes, bytes);
            }
        }

        [Fact]
        public void HashSecure_Parallel()
        {
            // Arrange
            const string word = "lorem ipsum";
            byte[] expectedBytes = Hasher.SecureHash(word);

            // Act
            Parallel.For(0, 10000, i =>
            {
                byte[] bytes = Hasher.SecureHash(word);

                Assert.Equal(expectedBytes, bytes);
            });
        }
    }
}
