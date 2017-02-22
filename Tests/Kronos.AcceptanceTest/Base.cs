﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Kronos.Client;
using Xunit;
using Xunit.Abstractions;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public abstract class Base
    {
        protected readonly ITestOutputHelper output;

        protected Base(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static readonly ConcurrentQueue<int> ports = new ConcurrentQueue<int>(Enumerable.Range(5000, 6000));

        [Fact]
        public async Task RunAsync()
        {
            int port;
            if (!ports.TryDequeue(out port))
            {
                throw new Exception("Available ports queue is empty");
            }

            output.WriteLine($"Creating client with port {port}");
            IKronosClient client = KronosClientFactory.FromLocalhost(port);

            output.WriteLine($"Creating server with port {port}");
            Task server = Server.Program.StartAsync(port);

            try
            {
                output.WriteLine("Processing internal test");
                await ProcessAsync(client).AwaitWithTimeout(5000);
                output.WriteLine("Processing internal finished");
            }
            catch (Exception ex)
            {
                output.WriteLine($"EXCEPTION: {ex}");
                Assert.False(true, ex.Message);
            }
            finally
            {
                try
                {
                    output.WriteLine("Stopping server");
                    Server.Program.Stop();

                    output.WriteLine("Waiting for server task to finish");
                    await server.AwaitWithTimeout(5000);

                    output.WriteLine("Server stopped");
                }
                catch (Exception ex)
                {
                    output.WriteLine($"EXCEPTION: {ex}");
                    Assert.False(true, ex.Message);
                }
            }
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
