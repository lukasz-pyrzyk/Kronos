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
                try
                {
                    return _nodes.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try
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

                    if (current is null)
                    {
                        _nodes.AddLast(item);
                    }
                    else
                    {
                        _nodes.AddBefore(current, item);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T Poll()
        {
            _lock.EnterWriteLock();
            try
            {
                if (_nodes.Count == 0)
                {
                    throw new InvalidOperationException("Queue is empty");
                }

                T value = _nodes.First.Value;
                _nodes.RemoveFirst();

                return value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }


        public T Peek()
        {
            _lock.EnterReadLock();
            try
            {
                if (_nodes.Count == 0)
                {
                    throw new InvalidOperationException("Queue is empty");
                }

                return _nodes.First.Value;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                _nodes.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _nodes.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T[] ToArray()
        {
            _lock.EnterReadLock();
            try
            {
                return _nodes.ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
