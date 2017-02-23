using BenchmarkDotNet.Attributes;
using Kronos.Core.Hashing;

namespace Benchmark.Types
{
    public class HashingAlgorithms
    {
        [Params(
            "abc",
            "Lorem ipsum dolor sit amet",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt")]
        public string Word { get; set; }

        [Benchmark]
        public int Classic()
        {
            return Word.GetHashCode();
        }

        [Benchmark]
        public int City()
        {
            return (int)CityHash.CityHash32(Word);
        }
    }
}
