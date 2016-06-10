﻿using System;
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

            using (IStorage storage = new InMemoryStorage())
            {
                storage.AddOrUpdate(key, sampleData);
            }
        }

        [Benchmark]
        public void AddAndUpdate()
        {
            string key = Guid.NewGuid().ToString();

            using (IStorage storage = new InMemoryStorage())
            {
                storage.AddOrUpdate(key, sampleData);
                storage.AddOrUpdate(key, sampleData); // update
            }
        }

        [Benchmark]
        public byte[] AddAndGet()
        {
            string key = Guid.NewGuid().ToString();

            byte[] result;
            using (IStorage storage = new InMemoryStorage())
            {
                storage.AddOrUpdate(key, sampleData);
                result = storage.TryGet(key);
            }

            return result;
        }

        [Benchmark]
        public void AddAndRemove()
        {
            string key = Guid.NewGuid().ToString();

            using (IStorage storage = new InMemoryStorage())
            {
                storage.AddOrUpdate(key, sampleData);
                storage.TryRemove(key);
            }
        }

        [Benchmark]
        public void Remove()
        {
            string key = Guid.NewGuid().ToString();

            using (IStorage storage = new InMemoryStorage())
            {
                storage.TryRemove(key);
            }
        }
    }
}
