﻿using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Kronos.Client;
using StackExchange.Redis;

namespace ClusterBenchmark.Benchmarks
{
    public class AppBenchmark
    {
        [Benchmark]
        public async Task Kronos()
        {
            var kronos = KronosClientFactory.CreateClient("cerber.cloudapp.net", 5000);

            string key = Guid.NewGuid().ToString();
            byte[] data = GetData();
            for (int i = 0; i < 1000; i++)
            {
                await kronos.InsertAsync(key, data, DateTime.UtcNow.AddSeconds(50));
            }
        }

        [Benchmark]
        public async Task Redis()
        {
            ConnectionMultiplexer redisCacheDistributor = ConnectionMultiplexer.Connect("cerber.cloudapp.net:6379");
            var redis = redisCacheDistributor.GetDatabase();
            string key = Guid.NewGuid().ToString();
            byte[] data = GetData();
            for (int i = 0; i < 1000; i++)
            {
                await redis.SetAddAsync(key, data);
            }
        }

        private static byte[] GetData()
        {
            var random = new Random();
            byte[] data = new byte[1024];
            random.NextBytes(data);
            return data;
        }
    }
}
