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
        private static readonly string[] Nodes = { "192.168.0.101", "192.168.0.102", "192.168.0.103" };
        
        protected IKronosClient KronosClient { get; private set; }
        protected IDatabase RedisClient { get; private set; }
        protected IServer[] RedisServers { get; private set; }

        [GlobalSetup]
        public void Setup()
        {
            string redisConnection = GetRedis();
            KronosClient = KronosClientFactory.FromConnectionString(Nodes);

            ConnectionMultiplexer redisCacheDistributor = ConnectionMultiplexer.Connect($"{redisConnection},allowAdmin=true");

            RedisServers = redisConnection.Split(',').Select(x => redisCacheDistributor.GetServer(x)).ToArray();
            RedisClient = redisCacheDistributor.GetDatabase();

            AdditionalSetup();
        }

        private string GetRedis()
        {
            StringBuilder builder = new StringBuilder();

            for (var i = 0; i < Nodes.Length; i++)
            {
                var node = Nodes[i];
                string nodeCs = $"{node}:6379";
                builder.Append(nodeCs);

                if (i != Nodes.Length - 1)
                {
                    builder.Append(',');
                }
            }

            return builder.ToString();
        }

        protected virtual void AdditionalSetup() { }
    }
}
