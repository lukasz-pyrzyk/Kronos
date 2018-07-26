using System;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    public readonly struct Element
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