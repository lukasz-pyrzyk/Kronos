using BenchmarkDotNet.Attributes;

namespace Benchmark.Hashes
{
    [Config(typeof(BaseConfig))]
    public class HashesBenchmark
    {
        private string dataStr;
        private Murmur2Unsafe _murmur2 = new Murmur2Unsafe();
        private Murmur3AUnsafe _murmur3 = new Murmur3AUnsafe();
        private XXHashUnsafe _xxHash = new XXHashUnsafe();

        [Setup]
        public void SetupData()
        {
            dataStr = new string('x', PayloadLength);
        }


        [Params(4, 16, 128)]
        public int PayloadLength { get; set; }

        [Benchmark]
        public uint Murmur2()
        {
            return _murmur2.Hash(dataStr);
        }

        [Benchmark]
        public uint Murmur3a()
        {
            return _murmur3.Hash(dataStr);
        }

        [Benchmark]
        public uint XXHash()
        {
            return _xxHash.Hash(dataStr);
        }

        [Benchmark(Baseline = true)]
        public uint FarmHash()
        {
            return Farmhash.Sharp.Farmhash.Hash32(dataStr);
        }
    }
}
