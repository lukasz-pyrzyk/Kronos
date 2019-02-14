using Kronos.Core.Configuration;
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
            Assert.Equal(args.Port, DefaultSettings.Port);
            Assert.Equal(args.Login, DefaultSettings.Login);
            Assert.Equal(args.Password, DefaultSettings.Password);
        }
    }
}
