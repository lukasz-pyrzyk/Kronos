using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ClusterBenchmark.Benchmarks
{
    public class AddAndRemove : Default
    {
        public const int Iterations = 1000;

        [Params(Size.Mb, Size.Mb * 4)]
        public int Kb { get; set; }

        [Params(1, 10, 100)]
        public int ParallelLevel { get; set; }
        
        private byte[] _data;

        protected override void AdditionalSetup()
        {
            _data = new byte[Size.Mb * 4];
            var random = new Random();
            random.NextBytes(_data);
        }

        [Benchmark]
        public void Kronos()
        {
            int elementsPerTask = Iterations / ParallelLevel;
            Parallel.For(0, ParallelLevel, async i =>
            {
                for (int j = 0; j < elementsPerTask; j++)
                {
                    string key = Guid.NewGuid().ToString();
                    await KronosClient.InsertAsync(key, _data, DateTime.UtcNow.AddMinutes(5))
                        .AwaitWithTimeout(5000)
                        .ConfigureAwait(false);

                    await KronosClient.DeleteAsync(key)
                        .AwaitWithTimeout(5000)
                        .ConfigureAwait(false);
                }
            });
        }

        [Benchmark]
        public void Redis()
        {
            int elementsPerTask = Iterations / ParallelLevel;
            Parallel.For(0, ParallelLevel, async i =>
            {
                for (int j = 0; j < elementsPerTask; j++)
                {
                    string key = Guid.NewGuid().ToString();
                    await RedisClient.SetAddAsync(key, _data)
                        .AwaitWithTimeout(5000)
                        .ConfigureAwait(false);

                    await RedisClient.KeyDeleteAsync(key)
                        .AwaitWithTimeout(5000)
                        .ConfigureAwait(false);
                }
            });
        }
    }

    public static class Extensions
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
