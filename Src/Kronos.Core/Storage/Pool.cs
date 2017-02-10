using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public class Pool<T> where T : new()
    {
        private const int Capacity = 100;

        private readonly Stack<T> _nodes = new Stack<T>(Capacity);

        public T Rent()
        {
            if (_nodes.Count == 0)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    _nodes.Push(new T());
                }
            }

            return _nodes.Pop();
        }

        public void Return(T node)
        {
            _nodes.Push(node);
        }
    }
}
