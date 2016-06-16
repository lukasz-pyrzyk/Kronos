using System;

namespace Kronos.Core.Storage
{
    public class NodeMetatada
    {
        public NodeMetatada(string key, DateTime expiryDate = default(DateTime))
        {
            Key = key;
            ExpiryDate = expiryDate;
        }

        public string Key { get; }
        public DateTime ExpiryDate { get; }

        public override string ToString()
        {
            return $"{Key}|{ExpiryDate:O}";
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
