using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Kronos.Core.Configuration;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class Auth : ISerializable<Auth>
    {
        public Memory<byte> HashedPassword { get; set; }

        public Memory<byte> Login { get; set; }

        public string GetLogin() => Login.Span.GetString();

        public static Auth FromCfg(AuthConfig cfg)
        {
            return new Auth
            {
                Login = cfg.Login.GetMemory(),
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
            return auth.Login.Span.SequenceEqual(Login.Span) &&
                   auth.HashedPassword.Span.SequenceEqual(HashedPassword.Span);
        }

        public void Write(ref SerializationStream stream)
        {
            stream.WriteWithPrefixLength(Login.Span);
            stream.WriteWithPrefixLength(HashedPassword.Span);
        }

        public void Read(ref DeserializationStream stream)
        {
            var login = stream.ReadMemoryWithLengthPrefix();
            Login = MemoryMarshal.AsMemory(login);

            var memory = stream.ReadMemoryWithLengthPrefix();
            HashedPassword = MemoryMarshal.AsMemory(memory);
        }
    }
}
