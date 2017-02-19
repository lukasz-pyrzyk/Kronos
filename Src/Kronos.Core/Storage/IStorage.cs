using System;

namespace Kronos.Core.Storage
{
    public interface IStorage : IDisposable
    {
        int Count { get; }

        void AddOrUpdate(string value, DateTime? expiryDate, byte[] obj);
        bool TryGet(string key, out byte[] obj);
        bool TryRemove(string key);
        bool Contains(string key);
        void Clear();
    }
}
