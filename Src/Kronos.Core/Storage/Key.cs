using System;
using Kronos.Core.Hashing;

namespace Kronos.Core.Storage
{
    public struct Key
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
    }
}
