using System;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    public readonly struct Element
    {
        public ByteString Data { get; }
        public DateTimeOffset? ExpiryDate { get; }

        public Element(ByteString data, DateTimeOffset? expiryDate = null)
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