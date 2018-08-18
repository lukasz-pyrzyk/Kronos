using System;
using System.Buffers;

namespace Kronos.Core.Storage
{
    public struct Element
    {
        public IMemoryOwner<byte> MemoryOwner { get; }
        public DateTimeOffset? ExpiryDate { get; private set; }

        public Element(IMemoryOwner<byte> memoryOwner, DateTimeOffset? expiryDate = null)
        {
            MemoryOwner = memoryOwner;
            ExpiryDate = expiryDate;
        }

        public void Expire()
        {
            ExpiryDate = DateTimeOffset.MinValue;
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