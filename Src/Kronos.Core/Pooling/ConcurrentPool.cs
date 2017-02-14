using System.Collections.Concurrent;

namespace Kronos.Core.Pooling
{
    public class ConcurrentPool<T> : IPool<T> where T : new()
    {
        private const int Capacity = 100;
        private readonly ConcurrentStack<T> _nodes = new ConcurrentStack<T>();

        public ConcurrentPool()
        {
            Allocate();
        }

        public int Count => _nodes.Count;

        public T Rent()
        {
            if (_nodes.Count == 0)
            {
                Allocate();
            }

            T item;
            _nodes.TryPop(out item);
            return item;
        }

        public T[] Rent(int count)
        {
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

        private void Allocate()
        {
            T[] items = new T[Capacity];
            for (int i = 0; i < Capacity; i++)
            {
                items[i] = new T();
            }

            _nodes.PushRange(items);
        }
    }
}
