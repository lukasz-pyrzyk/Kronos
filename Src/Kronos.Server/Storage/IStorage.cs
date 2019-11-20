using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Kronos.Server.Storage
{
    public interface IStorage : IEnumerable<Element>
    {
        int Count { get; }
        int ExpiringCount { get; }

        bool Add(string value, DateTimeOffset? expiryDate, ByteString obj);
        bool TryGet(string key, out ByteString obj);
        bool TryRemove(string key);
        bool Contains(string key);
        int Clear();
    }
}
