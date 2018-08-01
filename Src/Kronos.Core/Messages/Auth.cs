using System.Linq;
using Kronos.Core.Configuration;
using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class Auth
    {
        [Index(0)]
        public virtual byte[] HashedPassword { get; set; }

        [Index(1)]
        public virtual string Login { get; set; }

        public static Auth FromCfg(AuthConfig cfg)
        {
            return new Auth
            {
                Login = cfg.Login,
                HashedPassword = cfg.HashedPassword
            };
        }

        public static Auth Default()
        {
            return FromCfg(new AuthConfig
            {
                Login = Settings.DefaultLogin,
                Password = Settings.DefaultPassword
            });
        }

        public bool Authorize(Auth auth)
        {
            return auth.Login == Login && auth.HashedPassword.SequenceEqual(HashedPassword);
        }
    }
}
