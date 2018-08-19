using System;
using System.Linq;
using System.Runtime.InteropServices;
using Kronos.Core.Configuration;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct Auth : ISerializable<Auth>
    {
        public Memory<byte> HashedPassword { get; set; }

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
            return auth.Login == Login && auth.HashedPassword.Span.SequenceEqual(HashedPassword.Span);
        }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Login);
            stream.WriteWithPrefixLength(HashedPassword.Span);
        }

        public void Read(ref DeserializationStream stream)
        {
            Login = stream.ReadString();
            var memory =  stream.ReadMemoryWithLengthPrefix();
            HashedPassword = MemoryMarshal.AsMemory(memory);
        }
    }
}
