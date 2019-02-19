using EntryPoint;
using Kronos.Core.Configuration;
using Kronos.Core.Hashing;

namespace Kronos.Server
{
    public class SettingsArgs : BaseCliArguments
    {
        public SettingsArgs() : base("Kronos default settings")
        {
        }

        [OptionParameter("port", 'p')]
        public int Port { get; set; } = DefaultSettings.Port;

        [OptionParameter("login", 'l')]
        public string Login { get; set; } = DefaultSettings.Login;

        [OptionParameter("password")]
        public string Password { get; set; } = DefaultSettings.Password;

        public byte[] HashedPassword() => Hasher.SecureHash(Password);
    }
}
