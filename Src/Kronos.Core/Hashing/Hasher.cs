using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Kronos.Core.Hashing
{
    public static class Hasher
    {
        private static readonly ThreadLocal<SHA512> _sha512 = new ThreadLocal<SHA512>(SHA512.Create);

        public static int Hash(string word)
        {
            uint hash = Farmhash.Sharp.Farmhash.Hash32(word);
            int intHash = unchecked((int)hash);

            return intHash;
        }

        public static byte[] SecureHash(string word)
        {
            var bytes = Encoding.UTF8.GetBytes(word);
            return _sha512.Value.ComputeHash(bytes);
        }
    }
}
