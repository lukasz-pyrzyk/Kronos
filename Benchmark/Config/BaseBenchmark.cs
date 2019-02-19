using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Kronos.Client;
using Kronos.Core.Configuration;
using Kronos.Server;

namespace Benchmark.Config
{
    [Config(typeof(CustomConfig))]
    public abstract class BaseBenchmark : IDisposable
    {
        private const string Domain = "127.0.0.1";
        private static readonly string KronosConnection = Domain;
        private Task _server;

        protected IKronosClient KronosClient { get; private set; }

        [GlobalSetup]
        public void Setup()
        {
            try
            {
                _server = Task.Factory.StartNew(() => Kronos.Server.Program.Start(new SettingsArgs()),
                    TaskCreationOptions.LongRunning);

                while (!Kronos.Server.Program.IsWorking)
                {
                    LogMessage("Waiting for server warnup...");
                    Task.Delay(100).GetAwaiter().GetResult();

                    if (_server.IsFaulted)
                    {
                        LogMessage($"Server is faulted. Exception: {_server.Exception}");
                        throw _server.Exception;
                    }
                }
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
                Kronos.Server.Program.Stop();

                LogMessage("Waiting for server task to finish");
                _server?.GetAwaiter().GetResult();

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
