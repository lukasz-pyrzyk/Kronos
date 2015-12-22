using System;
using System.IO;

namespace Kronos.Shared.Network.Requests
{
    public class InsertRequest : Request
    {
        public DateTime ExpiryDate { get; }
        public string Key { get; }
        public Stream Stream { get; }
        public long StreamLength => Stream.Length;

        public InsertRequest(string key, Stream stream, DateTime expiryDate, string host, int port)
            : base(host, port)
        {
            Key = key;
            Stream = stream;
            ExpiryDate = expiryDate;
        }
    }
}
