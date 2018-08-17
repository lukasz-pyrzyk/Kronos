using System;
using System.Linq;
using Kronos.Core.Configuration;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct Auth : ISerializable<Auth>
    {
        public byte[] HashedPassword { get; set; }

        public string Login { get; set; }

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

        public void Write(SerializationStream stream)
        {
            stream.Write(Login);
            stream.WriteWithPrefixLength(HashedPassword);
        }

        public void Read(DeserializationStream stream)
        {
            Login = stream.ReadString();
            HashedPassword = stream.ReadBytesWithLengthPrefix().ToArray(); // todo allocation
        }
    }
}
