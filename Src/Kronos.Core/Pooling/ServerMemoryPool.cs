using System;
using System.Buffers;
using System.Collections.Generic;

namespace Kronos.Core.Pooling
{
    public struct RentedMemory : IMemoryOwner<byte>
    {
        private readonly ServerMemoryPool _pool;
        public IMemoryOwner<byte> Owner { get; }
        public Memory<byte> Memory => Owner.Memory;

        public RentedMemory(IMemoryOwner<byte> owner, ServerMemoryPool pool)
        {
            Owner = owner;
            _pool = pool;
        }

        public void Dispose()
        {
            _pool.Return(this);
        }
    }

    public class ServerMemoryPool
    {
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;
        private readonly List<RentedMemory> _items = new List<RentedMemory>();

        public IMemoryOwner<byte> Rent(int count)
        {
            var owner = _pool.Rent(count);
            var item = new RentedMemory(owner, this);
            _items.Add(item);
            return item;
        }

        public void Return(RentedMemory item)
        {
            item.Owner.Dispose();
            _items.Remove(item);
        }
    }
}
