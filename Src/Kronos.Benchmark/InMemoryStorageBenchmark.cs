using System;
using BenchmarkDotNet.Attributes;
using Kronos.Core.Storage;

namespace Kronos.Benchmark
{
    public class InMemoryStorageBenchmark
    {
        private static readonly byte[] sampleData;

        static InMemoryStorageBenchmark()
        {
            sampleData = new byte[1024];
            new Random().NextBytes(sampleData);
        }

        [Benchmark]
        public void Add()
        {
            string key = Guid.NewGuid().ToString();
            DateTime expiryDate = DateTime.MaxValue;

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                storage.AddOrUpdate(key, expiryDate, sampleData);
            }
        }

        [Benchmark]
        public void AddAndUpdate()
        {
            string key = Guid.NewGuid().ToString();
            DateTime expiryDate = DateTime.MaxValue;

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                storage.AddOrUpdate(key, expiryDate, sampleData);
                storage.AddOrUpdate(key, expiryDate, sampleData); // update
            }
        }

        [Benchmark]
        public byte[] AddAndGet()
        {
            string key = Guid.NewGuid().ToString();
            DateTime expiryDate = DateTime.MaxValue;


            byte[] result;
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                storage.AddOrUpdate(key, expiryDate, sampleData);
                result = storage.TryGet(key);
            }

            return result;
        }

        [Benchmark]
        public void AddAndRemove()
        {
            string key = Guid.NewGuid().ToString();
            DateTime expiryDate = DateTime.MaxValue;


            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                storage.AddOrUpdate(key, expiryDate, sampleData);
                storage.TryRemove(key);
            }
        }

        [Benchmark]
        public void Remove()
        {
            string key = Guid.NewGuid().ToString();

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                storage.TryRemove(key);
            }
        }
    }
}
