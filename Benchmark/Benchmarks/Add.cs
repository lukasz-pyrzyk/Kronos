using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ClusterBenchmark.Benchmarks
{
    public class Add : Default
    {
        [Params(Size.Mb, Size.Mb * 4)]
        public int Kb { get; set; }

        private byte[] _data;

        protected override void AdditionalSetup()
        {
            _data = new byte[Kb];
            var random = new Random();
            random.NextBytes(_data);

            RedisServer.FlushAllDatabases();
            KronosClient.ClearAsync().Wait();
        }

        [Benchmark]
        public async Task Kronos()
        {
            string key = Guid.NewGuid().ToString();
            await KronosClient.InsertAsync(key, _data, DateTime.UtcNow.AddSeconds(50));
        }

        [Benchmark]
        public async Task Redis()
        {
            string key = Guid.NewGuid().ToString();
            await RedisClient.SetAddAsync(key, _data);
        }
    }
}
