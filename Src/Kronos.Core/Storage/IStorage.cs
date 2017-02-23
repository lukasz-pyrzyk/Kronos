using System;

namespace Kronos.Core.Storage
{
    public interface IStorage : IDisposable
    {
        int Count { get; }

        bool Add(string value, DateTime? expiryDate, byte[] obj);
        void AddOrUpdate(string value, DateTime? expiryDate, byte[] obj);
        bool TryGet(string key, out byte[] obj);
        bool TryRemove(string key);
        bool Contains(string key);
        int Clear();
    }
}
