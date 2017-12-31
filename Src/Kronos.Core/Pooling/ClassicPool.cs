using System.Collections.Generic;

namespace Kronos.Core.Pooling
{
    public class ClassicPool<T> : Pool<T> where T : new()
    {
        private readonly int _allocateSize;
        private readonly Stack<T> _nodes = new Stack<T>();

        public ClassicPool(int preallocateCount = 100)
        {
            _allocateSize = preallocateCount;
            Allocate();
        }

        public override int Count => _nodes.Count;

        public override T Rent()
        {
            AllocateIfNecessary(1);

            return _nodes.Pop();
        }

        public override T[] Rent(int count)
        {
            AllocateIfNecessary(count);

            T[] items = new T[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = _nodes.Pop();
            }

            return items;
        }

        public override void Return(T node)
        {
            _nodes.Push(node);
        }

        public override void Return(T[] items)
        {
            foreach (T item in items)
            {
                _nodes.Push(item);
            }
        }

        protected sealed override void Allocate(int requestedCount = 0)
        {
            for (int i = 0; i < requestedCount + _allocateSize; i++)
            {
                _nodes.Push(new T());
            }
        }
    }
}
