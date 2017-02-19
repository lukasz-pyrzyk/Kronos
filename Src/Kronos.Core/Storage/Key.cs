using System;

namespace Kronos.Core.Storage
{
    public struct Key
    {
        private readonly int _hashCode;

        public string Value { get; }
        public DateTime? ExpiryDate { get; }

        public Key(string value, DateTime? expiryDate = null)
        {
            Value = value;
            ExpiryDate = expiryDate;
            _hashCode = Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value}|{ExpiryDate:s}";
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
