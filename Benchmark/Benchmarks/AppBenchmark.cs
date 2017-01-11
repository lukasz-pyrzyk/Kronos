using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ClusterBenchmark.Benchmarks
{
    public class AppBenchmark : DefaultBenchmark
    {
        [Params(1, 512, 1024, 4048)]
        public int Kb { get; set; }

        private byte[] data;
        
        protected override void AdditionalSetup()
        {
            data = new byte[Kb];
            var random = new Random();
            random.NextBytes(data);
        }

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
    }
}
