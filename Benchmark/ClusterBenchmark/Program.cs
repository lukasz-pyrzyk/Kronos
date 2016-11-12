using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark
{
    public class Program
    {
        private const int ExpirySecond = 100;


        public static void Main(string[] args)
        {
            int iterations, packageSize;
            bool parallelRun;

            if (args.Length != 3)
            {
                Console.WriteLine($"You need to specify three arguments: {nameof(iterations)}, {nameof(packageSize)}, {nameof(parallelRun)}");
            }

            iterations = int.Parse(args[0]);
            packageSize = int.Parse(args[1]);
            parallelRun = bool.Parse(args[2]);
            Console.WriteLine($"Starting benchmark with {iterations} iterations, {packageSize}mb data, parallel: {parallelRun}");

            int workersCount = parallelRun == false ? 1 : Environment.ProcessorCount;
            Task[] workers = new Task[workersCount];
            for (int i = 0; i < workersCount; i++)
            {
                workers[i] = StartAsync(iterations, packageSize);
            }

            Task.WaitAll(workers);
        }

        private static async Task StartAsync(int iterations, int packageSize)
        {
            string configPath = "KronosConfig.json";

            var watch = Stopwatch.StartNew();
            byte[] package = new byte[packageSize * 1024 * 1024];
            new Random().NextBytes(package);

            for (int i = 0; i < iterations; i++)
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

                if (fromServer.Length != package.Length)
                    throw new Exception(
                        $"Received message is invalid! Size should be {package.Length}, but wit {fromServer.Length}");

                Console.WriteLine($" DELETE - testing");
                await client.DeleteAsync(key);
                bool containsAfterDeletion = await client.ContainsAsync(key);
                Console.WriteLine($" DELETE - done (exists after deletion: {containsAfterDeletion})");
            }

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
