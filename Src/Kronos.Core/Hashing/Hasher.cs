using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Kronos.Core.Serialization;

namespace Kronos.Core.Hashing
{
    public static class Hasher
    {
        private static readonly ThreadLocal<SHA512> Sha512 = new ThreadLocal<SHA512>(SHA512.Create);

        public static int Hash(ReadOnlySpan<byte> bytes)
        {
            uint hash = Farmhash.Sharp.Farmhash.Hash32(bytes);
            int intHash = unchecked((int)hash);

            return intHash;
        }

        public static int Hash(string word)
        {
            Span<byte> bytes = stackalloc byte[word.Length];
            word.GetBytes(bytes);
            uint hash = Farmhash.Sharp.Farmhash.Hash32(bytes);
            int intHash = unchecked((int)hash);

            return intHash;
        }

        public static byte[] SecureHash(string word)
        {
            var bytes = Encoding.UTF8.GetBytes(word);
            return Sha512.Value.ComputeHash(bytes);
        }
    }
}
