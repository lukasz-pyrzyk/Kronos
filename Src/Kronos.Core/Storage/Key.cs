using System;
using Kronos.Core.Hashing;

namespace Kronos.Core.Storage
{
    public readonly struct Key : IEquatable<Key>
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

        public bool Equals(Key other)
        {
            return GetHashCode() == other.GetHashCode() && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Key && Equals((Key)obj);
        }
    }
}
