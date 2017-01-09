using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Kronos.Client;
using StackExchange.Redis;

namespace ClusterBenchmark.Benchmarks
{
    public class AppBenchmark : DefaultBenchmark
    {
        private static readonly byte[] data = GetData();

        [Benchmark]
        public async Task Kronos()
        {
            var kronos = KronosClientFactory.CreateClient(KronosConnection, 5000);

            string key = Guid.NewGuid().ToString();
            await kronos.InsertAsync(key, data, DateTime.UtcNow.AddSeconds(50));
        }

        [Benchmark]
        public async Task Redis()
        {
            ConnectionMultiplexer redisCacheDistributor = ConnectionMultiplexer.Connect(RedisConnection);
            var redis = redisCacheDistributor.GetDatabase();
            string key = Guid.NewGuid().ToString();
            await redis.SetAddAsync(key, data);
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
