using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Core.Configuration;
using Kronos.Server;
using Microsoft.Extensions.Hosting;
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
            IHost? program = null;
            try
            {
                const int port = DefaultSettings.Port;
                LogMessage($"Creating kronos client with port {port}");
                IKronosClient client = KronosClientFactory.FromLocalhost(port);

                LogMessage($"Creating server with port {port}");

                program = Program.CreateHostBuilder(GetSettings()).Build();
                await program.StartAsync();

                LogMessage("Waiting for server warn-up...");
                await Task.Delay(100);

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
                if (program != null)
                {
                    try
                    {
                        LogMessage("Stopping server");
                        await program.StopAsync();
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
                }
            }

            ResetEvent.Release();
        }

        protected virtual string[] GetSettings()
        {
            return Array.Empty<string>();
        }

        protected void LogMessage(string message)
        {
            Trace.WriteLine($"{GetType()} - {message}");
        }

        protected abstract Task ProcessAsync(IKronosClient client);
    }
}
