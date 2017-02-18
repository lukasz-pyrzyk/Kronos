using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using Kronos.Client;
using StackExchange.Redis;

namespace Benchmark.Config
{
    [Config(typeof(CustomConfig))]
    public abstract class ClusterBenchmark
    {
        private static readonly string[] _nodes = { "192.168.0.101", "192.168.0.102", "192.168.0.103" };
        
        protected IKronosClient KronosClient { get; private set; }
        protected IDatabase RedisClient { get; private set; }
        protected IServer RedisServer { get; private set; }

        [Setup]
        public void Setup()
        {
            string redisConnection = GetRedis();
            KronosClient = KronosClientFactory.FromConnectionString(_nodes);

            ConnectionMultiplexer redisCacheDistributor = ConnectionMultiplexer.Connect($"{redisConnection},allowAdmin=true");
            RedisServer = redisCacheDistributor.GetServer(redisConnection);
            RedisClient = redisCacheDistributor.GetDatabase();

            AdditionalSetup();
        }

        private string GetRedis()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var node in _nodes)
            {
                string nodeCs = $"{node}:6379,";
                builder.Append(nodeCs);
            }

            return builder.ToString();
        }

        protected virtual void AdditionalSetup() { }
    }
}
