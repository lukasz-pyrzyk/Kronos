using System;

namespace Kronos.Core.Storage
{
    public interface IStorage : IDisposable
    {
        int Count { get; }

        void AddOrUpdate(string key, DateTime expiryDate, byte[] obj);
        byte[] TryGet(string key);
        bool TryRemove(string key);
        bool Contains(string key);
    }
}
