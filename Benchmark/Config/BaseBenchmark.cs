using BenchmarkDotNet.Attributes;
using Kronos.Client;
using Kronos.Core.Configuration;
using StackExchange.Redis;

namespace Benchmark.Config
{
    [Config(typeof(CustomConfig))]
    public abstract class BaseBenchmark
    {
        private const string Domain = "127.0.0.1";

        private static readonly string KronosConnection = Domain;
        private static readonly string RedisConnection = $"{Domain}:6379";

        protected IKronosClient KronosClient { get; private set; }
        protected IDatabase RedisClient { get; private set; }
        protected IServer RedisServer { get; private set; }

        [Setup]
        public void Setup()
        {
            KronosClient = KronosClientFactory.FromIp(KronosConnection, Settings.DefaultPort);

            ConnectionMultiplexer redisCacheDistributor = ConnectionMultiplexer.Connect($"{RedisConnection},allowAdmin=true");
            RedisServer = redisCacheDistributor.GetServer(RedisConnection);
            RedisClient = redisCacheDistributor.GetDatabase();

            AdditionalSetup();
        }

        protected virtual void AdditionalSetup() { }
    }
}
