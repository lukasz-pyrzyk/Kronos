using System;

namespace Kronos.Shared.Network.Requests
{
    public class InsertRequest : Request
    {
        public DateTime ExpiryDate { get; }
        public string Key { get; }
        public byte[] Package { get; }
        public long StreamLength => Package.Length;

        public InsertRequest(string key, byte[] package, DateTime expiryDate)
        {
            Key = key;
            Package = package;
            ExpiryDate = expiryDate;
        }
    }
}
