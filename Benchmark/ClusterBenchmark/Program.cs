using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark
{
    public class Program
    {
        private const int ExpirySecond = 100;

        private static int Iterations;
        private static int PackageSize;
        private static bool ParallelRun;

        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine($"You need to specify three arguments: {nameof(Iterations)}, {nameof(PackageSize)}, {nameof(ParallelRun)}");
            }

            Iterations = int.Parse(args[0]);
            PackageSize = int.Parse(args[1]);
            ParallelRun = bool.Parse(args[2]);
            Console.WriteLine($"Starting benchmark with {Iterations} iterations, {PackageSize}mb data, parallel: {ParallelRun}");

            Task.WaitAll(StartAsync());
        }

        private static async Task StartAsync()
        {
            string configPath = "KronosConfig.json";

            var watch = Stopwatch.StartNew();
            byte[] package = new byte[PackageSize * 1024 * 1024];
            new Random().NextBytes(package);

            Func<Task> action = async () =>
            {
                IKronosClient client = KronosClientFactory.CreateClient(configPath);

                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                string key = Guid.NewGuid().ToString();
                DateTime expiryDate = DateTime.UtcNow.AddSeconds(ExpirySecond);

                Console.WriteLine($"ADD - testing");
                await client.InsertAsync(key, package, expiryDate);
                Console.WriteLine($" ADD - done (size: {package.Length})");

                Console.WriteLine($" COUNT - testing");
                int count = await client.CountAsync();
                Console.WriteLine($" COUNT - done (count: {count})");

                Console.WriteLine($" CONTAINS - testing");
                bool contains = await client.ContainsAsync(key);
                Console.WriteLine($"CONTAINS - done (exists: {contains})");

                Console.WriteLine($" GET - testing");
                byte[] fromServer = await client.GetAsync(key);
                Console.WriteLine($" GET - done (size: {fromServer.Length})");

                Console.WriteLine($" DELETE - testing");
                await client.DeleteAsync(key);
                bool containsAfterDeletion = await client.ContainsAsync(key);
                Console.WriteLine($" DELETE - done (exists after deletion: {containsAfterDeletion})");
            };

            if (ParallelRun)
                await RunParallel(action);
            else
                await Run(action);

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        private static async Task Run(Func<Task> action)
        {
            for (int i = 0; i < Iterations; i++)
            {
                await action.Invoke();
            }
        }

        private static async Task RunParallel(Func<Task> action)
        {
            Task[] tasks = new Task[Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                tasks[i] = action.Invoke();
            }

            await Task.WhenAll(tasks);
        }
    }
}
