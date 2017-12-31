using Benchmark.Config;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Hashes
{
    [Config(typeof(CustomConfig))]
    public class HashesBenchmark
    {
        private string _dataStr;
        private readonly Murmur2Unsafe _murmur2 = new Murmur2Unsafe();
        private readonly Murmur3AUnsafe _murmur3 = new Murmur3AUnsafe();
        private readonly XxHashUnsafe _xxHash = new XxHashUnsafe();

        [GlobalSetup]
        public void SetupData()
        {
            _dataStr = new string('x', PayloadLength);
        }


        [Params(4, 16, 128)]
        public int PayloadLength { get; set; }

        [Benchmark]
        public uint Murmur2()
        {
            return _murmur2.Hash(_dataStr);
        }

        [Benchmark]
        public uint Murmur3A()
        {
            return _murmur3.Hash(_dataStr);
        }

        [Benchmark]
        public uint XxHash()
        {
            return _xxHash.Hash(_dataStr);
        }

        [Benchmark(Baseline = true)]
        public uint FarmHash()
        {
            return Farmhash.Sharp.Farmhash.Hash32(_dataStr);
        }
    }
}
