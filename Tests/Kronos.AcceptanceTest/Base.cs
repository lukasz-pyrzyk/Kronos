using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Core.Configuration;
using Kronos.Server;
using NLog;
using NLog.Config;
using NLog.Targets;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public abstract class Base
    {
        private static readonly SemaphoreSlim ResetEvent = new SemaphoreSlim(1, 1);

        public abstract Task RunAsync();

        static Base()
        {
            Trace.Listeners.Add(new ConsoleLogger());
        }

        public async Task RunInternalAsync()
        {
            await ResetEvent.WaitAsync();
            Task server = null;
            try
            {
                const int port = Settings.DefaultPort;
                LogMessage($"Creating kronos client with port {port}");
                IKronosClient client = KronosClientFactory.FromLocalhost(port);

                LogMessage($"Creating server with port {port}");

                server = Task.Factory.StartNew(() => Program.Start(GetSettings(), GetLoggerConfig()),
                    TaskCreationOptions.LongRunning);

                while (!Program.IsWorking)
                {
                    LogMessage("Waiting for server warnup...");
                    await Task.Delay(100);

                    if (server.IsFaulted)
                    {
                        LogMessage($"Server is faulted. Exception: {server.Exception}");
                        throw server.Exception;
                    }
                }

                LogMessage("Processing internal test");
                await ProcessAsync(client).ConfigureAwait(true);
                LogMessage("Processing internal finished");
            }

            catch (Exception ex)
            {
                LogMessage($"EXCEPTION: {ex}");
                Assert.False(true, ex.Message);
            }
            finally
            {
                try
                {
                    LogMessage("Stopping server");
                    Program.Stop();

                    LogMessage("Waiting for server task to finish");
                    if (server != null)
                        await server.ConfigureAwait(true);

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
                    Assert.False(true, ex.Message);
                }

                ResetEvent.Release();
            }
        }

        protected virtual SettingsArgs GetSettings()
        {
            return new SettingsArgs();
        }

        private static LoggingConfiguration GetLoggerConfig()
        {
            var config = new LoggingConfiguration();
            config.AddTarget("console", new ConsoleTarget());
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, "console");
            return config;
        }

        protected void LogMessage(string message)
        {
            Trace.WriteLine($"{GetType()} - {message}");
        }

        protected abstract Task ProcessAsync(IKronosClient client);
    }
}
