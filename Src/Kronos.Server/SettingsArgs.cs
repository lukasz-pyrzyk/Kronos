using EntryPoint;

namespace Kronos.Server
{
    public class SettingsArgs : BaseCliArguments
    {
        public SettingsArgs() : base("Kronos default settings")
        {
        }

        [Option("port", 'p')]
        public int Port { get; set; } = 5000;

        [Option("login", 'l')]
        public string Login { get; set; } = "user";

        [Option("password")]
        public string Password { get; set; } = "password";
    }
}
