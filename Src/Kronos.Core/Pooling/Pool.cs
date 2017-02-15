using System.Collections.Generic;

namespace Kronos.Core.Pooling
{
    public class Pool<T> : IPool<T> where T : new()
    {
        private readonly int allocateSize;
        private readonly Stack<T> _nodes = new Stack<T>();

        public Pool(int preallocateCount = 100)
        {
            allocateSize = preallocateCount;
            Allocate();
        }

        public int Count => _nodes.Count;

        public T Rent()
        {
            AllocateIfNecessary(1);

            return _nodes.Pop();
        }

        public T[] Rent(int count)
        {
            AllocateIfNecessary(count);

            T[] items = new T[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = _nodes.Pop();
            }

            return items;
        }

        public void Return(T node)
        {
            _nodes.Push(node);
        }

        public void Return(T[] items)
        {
            foreach (T item in items)
            {
                _nodes.Push(item);
            }
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
            for (int i = 0; i < requestedCount + allocateSize; i++)
            {
                _nodes.Push(new T());
            }
        }
    }
}
