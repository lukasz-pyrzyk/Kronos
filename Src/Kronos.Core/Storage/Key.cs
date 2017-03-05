using System;
using Kronos.Core.Hashing;

namespace Kronos.Core.Storage
{
    public struct Key : IComparable<Key>
    {
        private readonly int _hashCode;

        public string Value { get; }
        public DateTime? ExpiryDate { get; }

        public Key(string value, DateTime? expiryDate = null)
        {
            Value = value;
            ExpiryDate = expiryDate;
            _hashCode = Hasher.Hash(value);
        }

        public bool IsExpired(DateTime date)
        {
            return ExpiryDate?.Ticks < date.Ticks;
        }

        public override string ToString()
        {
            return $"{Value}|{ExpiryDate:s}";
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public int CompareTo(Key other)
        {
            if (other.ExpiryDate == null)
                throw new InvalidOperationException("Key to compare is not expiring");

            if (ExpiryDate == null)
                throw new InvalidOperationException("Cannot compare, key is not expiring");

            return DateTime.Compare(ExpiryDate.Value, other.ExpiryDate.Value);
        }
    }
}
