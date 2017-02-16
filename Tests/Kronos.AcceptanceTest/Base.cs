using System;
using System.Threading.Tasks;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public abstract class Base
    {
        [Fact]
        public async Task RunAsync()
        {
            const int port = 5000;

            IKronosClient client = KronosClientFactory.FromLocalhost(port);
            Task server = Server.Program.StartAsync(port);

            await Task.Delay(2000);

            try
            {
                Task worker = ProcessAsync(client);
                await worker.AwaitWithTimeout(500);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
            finally
            {
                try
                {
                    Server.Program.Stop();
                    await server.AwaitWithTimeout(500);
                }
                catch (Exception ex)
                {
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
            if (await Task.WhenAny(task, Task.Delay(miliseconds)) != task)
            {
                Assert.False(true, "Timeout");
            }
            else
            {
                await task;
            }
        }
    }
}
