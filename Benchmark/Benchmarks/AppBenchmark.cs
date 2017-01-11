using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ClusterBenchmark.Benchmarks
{
    public class AppBenchmark : DefaultBenchmark
    {
        private static readonly byte[] data = GetData();

        [Benchmark]
        public async Task Kronos()
        {
            string key = Guid.NewGuid().ToString();
            await KronosClient.InsertAsync(key, data, DateTime.UtcNow.AddSeconds(50));
        }

        [Benchmark]
        public async Task Redis()
        {
            string key = Guid.NewGuid().ToString();
            await RedisClient.SetAddAsync(key, data);
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
