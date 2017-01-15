using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ClusterBenchmark.Benchmarks
{
    public class Contains : Default
    {
        [Benchmark]
        public async Task Kronos()
        {
            string key = Guid.NewGuid().ToString();
            await KronosClient.ContainsAsync(key);
        }

        [Benchmark]
        public async Task Redis()
        {
            string key = Guid.NewGuid().ToString();
            await RedisClient.SetContainsAsync(key, "");
        }
    }
}
