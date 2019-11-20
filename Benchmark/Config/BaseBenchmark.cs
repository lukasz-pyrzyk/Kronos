using BenchmarkDotNet.Attributes;
using Kronos.Client;
using Kronos.Core.Configuration;

namespace Benchmark.Config
{
    [Config(typeof(CustomConfig))]
    public abstract class BaseBenchmark
    {
        private const string KronosConnection = "127.0.0.1";

        protected IKronosClient KronosClient { get; private set; }

        [GlobalSetup]
        public void Setup()
        {
            KronosClient = KronosClientFactory.FromIp(KronosConnection, DefaultSettings.Port);
            AdditionalSetup();
        }

        protected virtual void AdditionalSetup() { }
    }
}
