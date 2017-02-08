using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using ClusterBenchmark.Utils;

namespace ClusterBenchmark.Benchmarks
{
    public class Contains : Default
    {
        [Benchmark]
        public async Task Kronos()
        {
            string key = Prepare.Key();
            await KronosClient.ContainsAsync(key);
        }

        [Benchmark]
        public async Task Redis()
        {
            string key = Prepare.Key();
            await RedisClient.SetContainsAsync(key, "");
        }
    }
}
