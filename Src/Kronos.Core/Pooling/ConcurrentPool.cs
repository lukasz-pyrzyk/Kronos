using System.Collections.Concurrent;

namespace Kronos.Core.Pooling
{
    public class ConcurrentPool<T> : Pool<T> where T : new()
    {
        private readonly int allocateSize;
        private readonly ConcurrentStack<T> _nodes = new ConcurrentStack<T>();

        public ConcurrentPool(int preallocateCount = 100)
        {
            allocateSize = preallocateCount;
            Allocate();
        }

        public override int Count => _nodes.Count;

        public override T Rent()
        {
            AllocateIfNecessary(1);

            _nodes.TryPop(out T item);
            return item;
        }

        public override T[] Rent(int count)
        {
            AllocateIfNecessary(count);

            T[] items = new T[count];
            for (int i = 0; i < count; i++)
            {
                _nodes.TryPop(out T item);
                items[i] = item;
            }

            return items;
        }

        public override void Return(T node)
        {
            _nodes.Push(node);
        }

        public override void Return(T[] items)
        {
            _nodes.PushRange(items);
        }

        protected sealed override void Allocate(int requestedCount = 0)
        {
            int size = requestedCount + allocateSize;
            T[] items = new T[size];
            for (int i = 0; i < size; i++)
            {
                items[i] = new T();
            }

            _nodes.PushRange(items);
        }
    }
}
