using System.Collections.Concurrent;

namespace Kronos.Core.Pooling
{
    public class ConcurrentPool<T> : IPool<T> where T : new()
    {
        private readonly int allocateSize;
        private readonly ConcurrentStack<T> _nodes = new ConcurrentStack<T>();

        public ConcurrentPool(int preallocateCount = 100)
        {
            allocateSize = preallocateCount;
            Allocate();
        }

        public int Count => _nodes.Count;

        public T Rent()
        {
            AllocateIfNecessary(1);

            T item;
            _nodes.TryPop(out item);
            return item;
        }

        public T[] Rent(int count)
        {
            AllocateIfNecessary(count);

            T[] items = new T[count];
            for (int i = 0; i < count; i++)
            {
                T item;
                _nodes.TryPop(out item);
                items[i] = item;
            }

            return items;
        }

        public void Return(T node)
        {
            _nodes.Push(node);
        }

        public void Return(T[] items)
        {
            _nodes.PushRange(items);
        }

        private void AllocateIfNecessary(int count)
        {
            if (_nodes.Count < count)
            {
                Allocate(count);
            }
        }

        private void Allocate(int requestedCount = 0)
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
