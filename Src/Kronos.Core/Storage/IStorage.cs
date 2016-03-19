namespace Kronos.Core.Storage
{
    public interface IStorage
    {
        int Count { get; }
        void AddOrUpdate(string key, byte[] obj);
        byte[] TryGet(string key);
        bool TryRemove(string key);
        void Clear();
    }
}
