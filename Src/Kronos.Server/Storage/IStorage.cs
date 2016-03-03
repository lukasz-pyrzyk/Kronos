namespace Kronos.Server.Storage
{
    internal interface IStorage
    {
        int Count { get; }
        void AddOrUpdate(string key, byte[] obj);
        byte[] TryGet(string key);
        void Clear();
    }
}
