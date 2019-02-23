using Benchmark.Types;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var switcher = BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(AddGetRemove),
                typeof(Contains),
            });

            switcher.Run(args);
        }
    }
}
