using System.Threading.Tasks;
using Benchmark.Config;
using Benchmark.Utils;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Types
{
    public class AddGetRemove : BaseBenchmark
    {
        [Params(Size.Kb * 100, Size.Kb * 500, Size.Mb, Size.Mb * 2)]
        public int PackageSize { get; set; }

        private byte[] _data;

        protected override void AdditionalSetup()
        {
            _data = Prepare.Bytes(PackageSize);

            Task kronos = KronosClient.ClearAsync();
            Task.WaitAll(kronos);
        }

        [Benchmark]
        public async Task Kronos()
        {
            string key = Prepare.Key();

            await KronosClient.InsertAsync(key, _data, null);
            await KronosClient.GetAsync(key);
            await KronosClient.DeleteAsync(key);
        }
    }
}
