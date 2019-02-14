using Kronos.Core.Hashing;

namespace Kronos.Client.Configuration
{
    public class AuthConfig
    {
        private byte[] _hashedPassword;

        public string Login { get; set; }

        public string Password { get; set; }

        public byte[] HashedPassword => _hashedPassword ?? (_hashedPassword = Hasher.SecureHash(Password));
    }
}
