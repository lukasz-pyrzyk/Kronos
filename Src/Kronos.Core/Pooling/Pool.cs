using System;
using System.Threading.Tasks;

namespace Kronos.Core.Pooling
{
    public abstract class Pool<T> where T : new()
    {
        public abstract int Count { get; }

        public abstract T Rent();
        public abstract T[] Rent(int count);
        public abstract void Return(T[] items);
        public abstract void Return(T item);

        public void Use(Action<T> action)
        {
            T item = Rent();
            try
            {
                action(item);
            }
            finally
            {
                Return(item);
            }
        }

        public async Task<TResult> UseAsync<TResult>(Func<T, Task<TResult>> action)
        {
            T item = Rent();
            TResult result;

            try
            {
                result = await action(item);
            }
            finally
            {
                Return(item);
            }

            return result;
        }

        public void Use(int count, Action<T[]> action)
        {
            T[] items = Rent(count);
            try
            {
                action(items);
            }
            finally
            {
                Return(items);
            }
        }

        public async Task<TResult> UseAsync<TResult>(int count, Func<T[], Task<TResult>> action)
        {
            T[] items = Rent(count);
            TResult result;

            try
            {
                result = await action(items);
            }
            finally
            {
                Return(items);
            }

            return result;
        }

        protected void AllocateIfNecessary(int requested)
        {
            if (Count < requested)
            {
                Allocate(requested);
            }
        }

        protected abstract void Allocate(int requestedElements = 0);
    }
}
