using System.Linq;
using System.Threading.Tasks;
using Benchmark.Config;
using Benchmark.Utils;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Types
{
    public class AddGetRemove_Cluster : ClusterBenchmark
    {
        [Params(Size.Kb * 100, Size.Kb * 500, Size.Mb, Size.Mb * 2)]
        public int PackageSize { get; set; }

        [Params(1, 2, 4, 8)]
        public int Clients { get; set; }

        private byte[] _data;

        protected override void AdditionalSetup()
        {
            _data = Prepare.Bytes(PackageSize);

            Task kronos = KronosClient.ClearAsync();
            Task[] redisTasks = RedisServers.Select(x => x.FlushAllDatabasesAsync()).ToArray();
            Task.WaitAll(redisTasks);
            Task.WaitAll(kronos);
        }

        [Benchmark]
        public void Kronos()
        {
            Parallel.For(1, Clients, _ =>
            {
                string key = Prepare.Key();

                KronosClient.InsertAsync(key, _data, null)
                    .GetAwaiter().GetResult();

                KronosClient.GetAsync(key)
                    .GetAwaiter().GetResult();

                KronosClient.DeleteAsync(key)
                    .GetAwaiter().GetResult();
            });
        }

        [Benchmark(Baseline = true)]
        public void Redis()
        {
            Parallel.For(0, Clients, _ =>
            {
                string key = Prepare.Key();

                RedisClient.StringSet(key, _data);
                RedisClient.StringGet(key);
                RedisClient.KeyDelete(key);
            });
        }
    }
}
