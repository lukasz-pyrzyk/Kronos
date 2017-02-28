using Xunit;

namespace Kronos.Server.Tests
{
    public class SettingsArgsTests
    {
        [Fact]
        public void Ctor_AssingsDefaultValues()
        {
            // Arrange and act
            var args = new SettingsArgs();

            // Assert
            Assert.Equal(args.Port, 5000);
            Assert.Equal(args.Login, "user");
            Assert.Equal(args.Password, "password");
        }
    }
}
