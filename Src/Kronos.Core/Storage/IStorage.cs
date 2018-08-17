﻿using System;
using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public interface IStorage : IDisposable, IEnumerable<Element>
    {
        int Count { get; }
        int ExpiringCount { get; }

        bool Add(string value, DateTimeOffset? expiryDate, ReadOnlyMemory<byte> data);
        bool TryGet(string key, out ReadOnlyMemory<byte> data);
        bool TryRemove(string key);
        bool Contains(string key);
        int Clear();
    }
}
