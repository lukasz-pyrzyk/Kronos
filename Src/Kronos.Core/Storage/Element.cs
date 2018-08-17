using System;

namespace Kronos.Core.Storage
{
    public readonly struct Element
    {
        public byte[] Data { get; }
        public DateTimeOffset? ExpiryDate { get; }

        public Element(byte[] data, DateTimeOffset? expiryDate = null)
        {
            Data = data;
            ExpiryDate = expiryDate;
        }

        public bool IsExpiring => ExpiryDate.HasValue;

        public bool IsExpired()
        {
            return IsExpired(DateTimeOffset.UtcNow);
        }

        public bool IsExpired(DateTimeOffset date)
        {
            return ExpiryDate?.Ticks < date.Ticks;
        }
    }
}