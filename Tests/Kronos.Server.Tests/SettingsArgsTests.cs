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
            Assert.Equal(args.Port, Settings.DefaultPort);
            Assert.Equal(args.Login, Settings.DefaultLogin);
            Assert.Equal(args.Password, Settings.DefaultPassword);
        }
    }
}
