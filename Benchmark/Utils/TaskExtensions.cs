using System.Threading.Tasks;

namespace Benchmark.Utils
{
    public static class TaskExtensions
    {
        public static async Task AwaitWithTimeout(this Task task, int timeout, int waitOnTimeout = 1000)
        {
            if (await Task.WhenAny(task, Task.Delay(timeout)) != task)
            {
                await Task.Delay(waitOnTimeout);
            }
            else
            {
                await task;
            }
        }
    }
}