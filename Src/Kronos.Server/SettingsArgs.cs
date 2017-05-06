using EntryPoint;
using Kronos.Core.Configuration;

namespace Kronos.Server
{
    public class SettingsArgs : BaseCliArguments
    {
        public SettingsArgs() : base("Kronos default settings")
        {
        }

        [Option("port", 'p')]
        public int Port { get; set; } = Settings.DefaultPort;

        [Option("login", 'l')]
        public string Login { get; set; } = Settings.DefaultLogin;

        [Option("password")]
        public string Password { get; set; } = Settings.DefaultPassword;
    }
}
