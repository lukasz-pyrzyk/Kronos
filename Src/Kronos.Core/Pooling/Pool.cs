using System.Collections.Generic;

namespace Kronos.Core.Pooling
{
    public class Pool<T> : IPool<T> where T : new()
    {
        private const int Capacity = 100;
        private readonly Stack<T> _nodes = new Stack<T>(Capacity);

        public Pool()
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

            return _nodes.Pop();
        }

        public T[] Rent(int count)
        {
            throw new System.NotImplementedException();
        }

        public void Return(T node)
        {
            _nodes.Push(node);
        }

        public void Return(T[] items)
        {
            throw new System.NotImplementedException();
        }

        private void Allocate()
        {
            for (int i = 0; i < Capacity; i++)
            {
                _nodes.Push(new T());
            }
        }
    }
}
