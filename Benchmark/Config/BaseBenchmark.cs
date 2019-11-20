using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Kronos.Client;
using Kronos.Core.Configuration;
using Microsoft.Extensions.Hosting;

namespace Benchmark.Config
{
    [Config(typeof(CustomConfig))]
    public abstract class BaseBenchmark : IDisposable
    {
        private const string KronosConnection = "127.0.0.1";
        private IHost _server;

        protected IKronosClient KronosClient { get; private set; }

        [GlobalSetup]
        public void Setup()
        {
            try
            {
                _server = Kronos.Server.Program.CreateHostBuilder(new string[0]).Build();
                _server.StartAsync().GetAwaiter().GetResult();

                // wait for warmup
                Task.Delay(100).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LogMessage($"EXCEPTION: {ex}");
            }
            KronosClient = KronosClientFactory.FromIp(KronosConnection, DefaultSettings.Port);
            AdditionalSetup();
        }

        private void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        protected virtual void AdditionalSetup() { }

        public void Dispose()
        {
            try
            {
                LogMessage("Stopping server");
                _server.StopAsync().GetAwaiter().GetResult();
                LogMessage("Server stopped");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.InnerExceptions)
                {
                    LogMessage(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                LogMessage($"EXCEPTION: {ex}");
            }
        }
    }
}
