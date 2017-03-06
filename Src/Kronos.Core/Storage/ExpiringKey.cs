using System;

namespace Kronos.Core.Storage
{
    internal struct ExpiringKey : IComparable<ExpiringKey>
    {
        public Key Key { get; }
        public DateTime ExpiryDate { get; }

        public ExpiringKey(Key key, DateTime expiryDate)
        {
            // todo write test
            Key = key;
            ExpiryDate = expiryDate;
        }

        public bool IsExpired()
        {
            // todo write test
            return IsExpired(DateTime.UtcNow);
        }

        public bool IsExpired(DateTime date)
        {
            // todo write test
            return ExpiryDate.Ticks < date.Ticks;
        }

        public int CompareTo(ExpiringKey other)
        {
            // todo write test
            return other.ExpiryDate.CompareTo(ExpiryDate);
        }
    }
}