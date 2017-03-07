using Kronos.Core.Hashing;

namespace Kronos.Core.Storage
{
    public struct Key
    {
        private readonly int _hashCode;

        public string Name { get; }

        public Key(string name)
        {
            Name = name;
            _hashCode = Hasher.Hash(name);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
