using System;
using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public interface IStorage : IDisposable, IEnumerable<Element>
    {
        int Count { get; }
        int ExpiringCount { get; }

        bool Add(ReadOnlyMemory<byte> value, DateTimeOffset? expiryDate, ReadOnlyMemory<byte> memory);
        bool TryGet(ReadOnlyMemory<byte> key, out ReadOnlyMemory<byte> data);
        bool TryRemove(ReadOnlyMemory<byte> key);
        bool Contains(ReadOnlyMemory<byte> key);
        int Clear();
    }
}
