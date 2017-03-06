using System;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    internal struct Element
    {
        public ByteString Data { get; }
        public DateTime? ExpiryDate { get; }

        public Element(ByteString data, DateTime? expiryDate = null)
        {
            // todo write test
            Data = data;
            ExpiryDate = expiryDate;
        }

        // todo write test
        public bool IsExpiring => ExpiryDate.HasValue;

        public bool IsExpired()
        {
            // todo write test
            return IsExpired(DateTime.UtcNow);
        }

        public bool IsExpired(DateTime date)
        {
            // todo write test
            return ExpiryDate?.Ticks < date.Ticks;
        }
    }
}