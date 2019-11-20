using System;

namespace Kronos.Core.Storage
{
    public readonly struct ExpiringKey : IComparable<ExpiringKey>, IEquatable<ExpiringKey>
    {
        public Key Key { get; }
        public DateTimeOffset ExpiryDate { get; }

        public ExpiringKey(Key key, DateTimeOffset expiryDate)
        {
            Key = key;
            ExpiryDate = expiryDate;
        }

        public bool IsExpired()
        {
            return IsExpired(DateTimeOffset.UtcNow);
        }

        public bool IsExpired(DateTimeOffset date)
        {
            return ExpiryDate.Ticks < date.Ticks;
        }

        public int CompareTo(ExpiringKey other)
        {
            return other.ExpiryDate.CompareTo(ExpiryDate);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ExpiringKey key) return Equals(key);
            return false;
        }

        public bool Equals(ExpiringKey expiringKey)
        {
            return Key.Equals(expiringKey.Key);
        }

        public override string ToString() => $"{Key.Name}|{ExpiryDate:g}";
    }
}