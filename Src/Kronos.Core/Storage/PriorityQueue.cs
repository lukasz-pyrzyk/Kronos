using System;
using System.Collections;
using System.Collections.Generic;

namespace Kronos.Core.Storage
{
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private readonly LinkedList<T> _nodes = new LinkedList<T>();

        public int Count => _nodes.Count;

        public void Add(T item)
        {
            // todo write test
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
            // todo write test
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
            // todo write test
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return _nodes.First.Value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _nodes.GetEnumerator(); // todo write test
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // todo write test
        }

        public void Remove(T item)
        {
            _nodes.Remove(item); // todo write test
        }

        public void Clear()
        {
            _nodes.Clear(); // todo write test
        }
    }
}
