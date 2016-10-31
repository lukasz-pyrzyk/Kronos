using System;

namespace Kronos.Core.Storage
{
    public struct ExpiryDate
    {
        public long Ticks { get; }

        public ExpiryDate(long ticks)
        {
            Ticks = ticks;
        }

        public override string ToString()
        {
            return ((DateTime) this).ToString("s");
        }

        public static implicit operator ExpiryDate(DateTime v)
        {
            return new ExpiryDate(v.Ticks);
        }

        public static implicit operator DateTime(ExpiryDate v)
        {
            return new DateTime(v.Ticks);
        }
    }
}
