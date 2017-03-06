using System;
using Google.Protobuf;
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

    public struct ExpiringKey : IComparable<ExpiringKey>
    {
        public Key Key { get; }
        public DateTime ExpiryDate { get; }

        public ExpiringKey(Key key, DateTime expiryDate)
        {
            Key = key;
            ExpiryDate = expiryDate;
        }

        public bool IsExpired()
        {
            return IsExpired(DateTime.UtcNow);
        }

        public bool IsExpired(DateTime date)
        {
            return ExpiryDate.Ticks < date.Ticks;
        }

        public int CompareTo(ExpiringKey other)
        {
            return other.ExpiryDate.CompareTo(ExpiryDate);
        }
    }

    public struct Element
    {
        public ByteString Data { get; }
        public DateTime? ExpiryDate { get; }

        public Element(ByteString data, DateTime? expiryDate = null)
        {
            Data = data;
            ExpiryDate = expiryDate;
        }

        public bool IsExpiring => ExpiryDate.HasValue;

        public bool IsExpired()
        {
            return IsExpired(DateTime.UtcNow);
        }

        public bool IsExpired(DateTime date)
        {
            return ExpiryDate?.Ticks < date.Ticks;
        }
    }
}
