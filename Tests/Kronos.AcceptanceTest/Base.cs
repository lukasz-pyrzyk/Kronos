using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Server;
using NLog;
using NLog.Config;
using NLog.Targets;
using Xunit;

namespace Kronos.AcceptanceTest
{
    public abstract class Base
    {
        public abstract Task RunAsync();

        static Base()
        {
            Trace.Listeners.Add(new ConsoleLogger());
        }

        public async Task RunInternalAsync()
        {
            const int port = 5000;

            LogMessage($"Creating kronos client with port {port}");
            IKronosClient client = KronosClientFactory.FromLocalhost(port);

            LogMessage($"Creating server with port {port}");

            var loggerConfig = GetLoggerConfig();
            Task server = Server.Program.StartAsync(GetSettings(), loggerConfig);
            while (!Server.Program.IsWorking)
            {
                LogMessage("Waiting for server warnup...");
                await Task.Delay(100);

                if (server.IsFaulted)
                {
                    throw server.Exception;
                }
            }

            try
            {
                LogMessage("Processing internal test");
                await ProcessAsync(client).AwaitWithTimeout(5000);
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
                    Server.Program.Stop();

                    LogMessage("Waiting for server task to finish");
                    await server;

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

        protected virtual CliArguments GetSettings()
        {
            return new CliArguments();
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

    public static class TaskExtensions
    {
        public static async Task AwaitWithTimeout(this Task task, int miliseconds)
        {
            if (await Task.WhenAny(task, Task.Delay(miliseconds)) == task)
            {
                await task;
            }
        }
    }
}
