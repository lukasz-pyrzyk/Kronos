using BenchmarkDotNet.Attributes;
using Kronos.Client;
using StackExchange.Redis;

namespace ClusterBenchmark.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public abstract class DefaultBenchmark
    {
        private const string Domain = "192.168.0.1";

        private static readonly string KronosConnection = Domain;
        private static readonly string RedisConnection = $"{Domain}:6379";

        protected IKronosClient KronosClient { get; private set; }
        protected IDatabase RedisClient { get; private set; }

        [Setup]
        public void Setup()
        {
            KronosClient = KronosClientFactory.FromIp(KronosConnection, 5000);

            ConnectionMultiplexer redisCacheDistributor = ConnectionMultiplexer.Connect(RedisConnection);
            RedisClient = redisCacheDistributor.GetDatabase();

            AdditionalSetup();
        }

        protected virtual void AdditionalSetup() { }
    }
}
