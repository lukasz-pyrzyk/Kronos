using System.Threading.Tasks;
using Kronos.Core.Hashing;
using Xunit;

namespace Kronos.Core.Tests.Hashing
{
    public class HasherTests
    {
        [Fact]
        public void HashSecure()
        {
            // Arrange
            const string word = "lorem ipsum";
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
