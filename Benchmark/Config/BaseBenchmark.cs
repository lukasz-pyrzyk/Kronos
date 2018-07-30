using BenchmarkDotNet.Attributes;
using Kronos.Client;
using Kronos.Core.Configuration;

namespace Benchmark.Config
{
    [Config(typeof(CustomConfig))]
    public abstract class BaseBenchmark
    {
        private const string Domain = "127.0.0.1";
        private static readonly string KronosConnection = Domain;

        protected IKronosClient KronosClient { get; private set; }

        [GlobalSetup]
        public void Setup()
        {
            KronosClient = KronosClientFactory.FromIp(KronosConnection, Settings.DefaultPort);

            AdditionalSetup();
        }

        protected virtual void AdditionalSetup() { }
    }
}
