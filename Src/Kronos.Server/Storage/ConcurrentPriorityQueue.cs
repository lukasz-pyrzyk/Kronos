using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kronos.Server.Storage
{
    class ConcurrentPriorityQueue<T> where T : IComparable<T>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly LinkedList<T> _nodes = new LinkedList<T>();

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                int count = _nodes.Count;
                _lock.ExitReadLock();
                return count;
            }
        }

        public void Add(T item)
        {
            _lock.EnterWriteLock();
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

                if (current is null)
                {
                    _nodes.AddLast(item);
                }
                else
                {
                    _nodes.AddBefore(current, item);
                }
            }
            _lock.ExitWriteLock();
        }

        public T Poll()
        {
            _lock.EnterWriteLock();
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            T value = _nodes.First.Value;
            _nodes.RemoveFirst();

            _lock.ExitWriteLock();
            return value;
        }


        public T Peek()
        {
            _lock.EnterReadLock();
            if (_nodes.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var item = _nodes.First.Value;
            _lock.ExitReadLock();
            return item;
        }

        public void Remove(T item)
        {
            _lock.EnterWriteLock();
            _nodes.Remove(item);
            _lock.ExitWriteLock();
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            _nodes.Clear();
            _lock.ExitWriteLock();
        }

        public T[] ToArray()
        {
            _lock.EnterReadLock();
            var array = _nodes.ToArray();
            _lock.ExitReadLock();
            return array;
        }
    }
}
