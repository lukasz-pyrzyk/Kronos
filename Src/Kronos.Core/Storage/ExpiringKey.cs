using System;

namespace Kronos.Core.Storage
{
    internal struct ExpiringKey : IComparable<ExpiringKey>
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
}