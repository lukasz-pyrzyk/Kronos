using Kronos.Core.Hashing;

namespace Kronos.Core.Configuration
{
    public static class DefaultSettings
    {
        public static readonly string Login = "user";
        public static readonly string Password = "password";
        public static readonly byte[] HashedPassword = Hasher.SecureHash(Password);

        public const int Port = 44000;
        public const int CleanupTimeMs = 5000;
    }
}
