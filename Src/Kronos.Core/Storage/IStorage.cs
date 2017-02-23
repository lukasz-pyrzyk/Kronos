using System;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    public interface IStorage : IDisposable
    {
        int Count { get; }

        bool Add(string value, DateTime? expiryDate, ByteString obj);
        void AddOrUpdate(string value, DateTime? expiryDate, ByteString obj);
        bool TryGet(string key, out ByteString obj);
        bool TryRemove(string key);
        bool Contains(string key);
        int Clear();
    }
}
