namespace Kronos.Core.Hashing
{
    public static class Hasher
    {
        public static int Hash(string word)
        {
            uint hash = Farmhash.Hash32(word);
            int intHash = unchecked((int)hash);

            return intHash;
        }
    }
}
