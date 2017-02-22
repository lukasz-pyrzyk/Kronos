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
                typeof(AddAndGet),
                typeof(AddAndRemove),
                typeof(Contains),
                typeof(ClusterAddAndGet),
                typeof(ClusterAddAndRemove),
                typeof(ClusterContains),
                typeof(HashingAlgorithms),
                typeof(ProtobufVsProtobufNet)
            });

            switcher.Run(args);
        }
    }
}
