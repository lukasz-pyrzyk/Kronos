using BenchmarkDotNet.Attributes;

namespace ClusterBenchmark.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public abstract class DefaultBenchmark
    {
        private const string Domain = "localhost";

        public static readonly string KronosConnection = Domain;
        public static readonly string RedisConnection = $"{Domain}:6379";
    }
}
