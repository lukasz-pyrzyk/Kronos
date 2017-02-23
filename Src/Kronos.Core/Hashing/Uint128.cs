using System;

namespace Kronos.Core.Hashing
{
    public struct Uint128 : IEquatable<Uint128>
    {
        public Uint128(ulong low, ulong high)
                : this()
        {
            Low = low;
            High = high;
        }

        public ulong Low { get; set; }

        public ulong High { get; set; }

        public bool Equals(Uint128 other)
        {
            return Low == other.Low && High == other.High;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Uint128 && Equals((Uint128)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Low.GetHashCode() * 397) ^ High.GetHashCode();
            }
        }
    }
}
