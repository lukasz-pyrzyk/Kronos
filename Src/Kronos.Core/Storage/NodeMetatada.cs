using System;

namespace Kronos.Core.Storage
{
    public class NodeMetatada
    {
        private readonly int _hashCode;

        public string Key { get; }
        public ExpiryDate ExpiryDate { get; }

        public NodeMetatada(string key, DateTime expiryDate = default(DateTime))
        {
            Key = key;
            ExpiryDate = expiryDate;
            _hashCode = key.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Key}|{ExpiryDate}";
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
