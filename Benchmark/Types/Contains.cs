using System.Threading.Tasks;
using Benchmark.Config;
using Benchmark.Utils;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Types
{
    public class Contains : BaseBenchmark
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
