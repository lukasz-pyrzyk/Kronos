using System;
using System.Buffers;

namespace Kronos.Core.Storage
{
    public struct Element
    {
        public int Length { get; set; }
        public IMemoryOwner<byte> MemoryOwner { get; }
        public Memory<byte> Memory => MemoryOwner.Memory.Slice(0, Length);
        public DateTimeOffset? ExpiryDate { get; private set; }

        public Element(IMemoryOwner<byte> memoryOwner, int length, DateTimeOffset? expiryDate = null)
        {
            MemoryOwner = memoryOwner;
            Length = length;
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