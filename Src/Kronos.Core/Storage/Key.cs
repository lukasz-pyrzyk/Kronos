using Kronos.Core.Hashing;

namespace Kronos.Core.Storage
{
    public struct Key
    {
        private readonly int _hashCode;

        public string Value { get; }
        
        public Key(string value)
        {
            Value = value;
            _hashCode = Hasher.Hash(value);
        }

        public override string ToString()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
