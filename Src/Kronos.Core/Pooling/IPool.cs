namespace Kronos.Core.Pooling
{
    public interface IPool<T> where T : new()
    {
        int Count { get; }
        T Rent();
        T[] Rent(int count);
        void Return(T item);
        void Return(T[] items);
    }
}
