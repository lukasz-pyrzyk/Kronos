using System;
using System.IO;

namespace Kronos.Shared.Network.Models
{
    public class SocketRequest
    {
        public DateTime ExpiryDate { get; }
        public string Key { get; }
        public Stream Stream { get; }
        public long StreamLength => Stream.Length;

        public SocketRequest(string key, Stream stream, DateTime expiryDate)
        {
            Key = key;
            Stream = stream;
            ExpiryDate = expiryDate;
        }
    }
}
