using EntryPoint;
using Kronos.Core.Configuration;

namespace Kronos.Server
{
    public class SettingsArgs : BaseCliArguments
    {
        public SettingsArgs() : base("Kronos default settings")
        {
        }

        [OptionParameter("port", 'p')]
        public int Port { get; set; } = Settings.DefaultPort;

        [OptionParameter("login", 'l')]
        public string Login { get; set; } = Settings.DefaultLogin;

        [OptionParameter("password")]
        public string Password { get; set; } = Settings.DefaultPassword;
    }
}
