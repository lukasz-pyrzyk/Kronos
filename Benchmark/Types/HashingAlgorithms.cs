using BenchmarkDotNet.Attributes;
using Kronos.Core.Hashing;

namespace Benchmark.Types
{
    public class HashingAlgorithms
    {
        [Params(
            "030B4A82-1B7C-11CF-9D53-00AA003C9CB6",
            "Lorem ipsum",
            "Lorem ipsum dolor sit amet",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt")]
        public string Word { get; set; }

        private static readonly Murmur3AUnsafe murmur = new Murmur3AUnsafe();

        [Benchmark]
        public uint Murmur3()
        {
            return murmur.Hash(Word);
        }

        [Benchmark]
        public uint City()
        {
            return CityHash.CityHash32(Word);
        }

        [Benchmark]
        public uint Farm()
        {
            return Farmhash.Hash32(Word);
        }
    }
}
