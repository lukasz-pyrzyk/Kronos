using System;
using Kronos.Core.Hashing;

namespace Kronos.Core.Storage
{
    public readonly struct Key : IEquatable<Key>
    {
        private readonly int _hashCode;

        public ReadOnlyMemory<byte> Name { get; }

        public Key(ReadOnlyMemory<byte> name)
        {
            Name = name;
            _hashCode = Hasher.Hash(Name.Span);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(Key other)
        {
            return GetHashCode() == other.GetHashCode() && Name.Span.SequenceEqual(other.Name.Span);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Key key && Equals(key);
        }
    }
}
