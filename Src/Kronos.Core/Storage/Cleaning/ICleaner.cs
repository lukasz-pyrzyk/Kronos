namespace Kronos.Core.Storage.Cleaning
{
    internal interface ICleaner
    {
        void Clear(PriorityQueue<ExpiringKey> expiringKeys, IStorage storage);
    }
}