using System;
using System.Collections;
using System.Collections.Generic;

namespace Kronos.Server.Storage
{
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private readonly LinkedList<T> _nodes = new LinkedList<T>();

        public int Count => _nodes.Count;

        public void Add(T item)
        {
            if (_nodes.Count == 0)
            {
                _nodes.AddLast(item);
            }
            else
            {
                var current = _nodes.First;

                while (current != null && current.Value.CompareTo(item) > 0)
                {
                    current = current.Next;
                }

                if (current == null)
                {
                    _nodes.AddLast(item);
                }
                else
                {
                    _nodes.AddBefore(current, item);
                }
            }
        }

        public T Poll()
        {
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            T value = _nodes.First.Value;
            _nodes.RemoveFirst();

            return value;
        }


        public T Peek()
        {
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return _nodes.First.Value;
        }

        public void Remove(T item)
        {
            _nodes.Remove(item);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
