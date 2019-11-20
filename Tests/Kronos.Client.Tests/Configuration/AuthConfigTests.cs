using Kronos.Client.Configuration;
using Kronos.Core.Hashing;
using Xunit;

namespace Kronos.Client.Tests.Configuration
{
    public class AuthConfigTests
    {
        [Fact]
        public void HashedPassword_ReturnsHashedPassword()
        {
            // Arrange
            var auth = new AuthConfig();
            const string password = "lorem ipsum";
            byte[] expectedBytes = Hasher.SecureHash(password);
            auth.Password = password;

            // Act
            byte[] bytes = auth.HashedPassword;

            // Assert
            Assert.Equal(expectedBytes, bytes);
        }
    }
}
