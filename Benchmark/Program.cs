using BenchmarkDotNet.Running;
using ClusterBenchmark.Benchmarks;

namespace ClusterBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<AppBenchmark>();
        }
    }
}
