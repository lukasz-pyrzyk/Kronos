namespace Kronos.Core.Storage
{
    public struct Key
    {
        private readonly int _hashCode;

        public string Value { get; }
        public ExpiryDate ExpiryDate { get; }

        public Key(string value, ExpiryDate expiryDate = default(ExpiryDate))
        {
            Value = value;
            ExpiryDate = expiryDate;
            _hashCode = Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value}|{ExpiryDate}";
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
