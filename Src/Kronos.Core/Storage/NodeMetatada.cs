using System;

namespace Kronos.Core.Storage
{
    public class NodeMetatada
    {
        private int _hashCode;

        public string Key { get; private set; }
        public ExpiryDate ExpiryDate { get; private set; }

        public NodeMetatada()
        {
        }

        public NodeMetatada(string key, ExpiryDate expiryDate = default(ExpiryDate))
        {
            Reuse(key, expiryDate);
        }

        public void Reuse(string key, DateTime expiryDate = default(DateTime))
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
