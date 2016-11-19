using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark
{
    public class Program
    {
        private const int ExpirySecond = 100;
        private static ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();

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
            Console.WriteLine($"{DateTime.Now.ToString("O")} Starting benchmark with {iterations} iterations, {packageSize}mb data, parallel: {parallelRun}");

            int workersCount = parallelRun == false ? 1 : 2;
            Task[] workers = new Task[workersCount];

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < workersCount; i++)
            {
                workers[i] = StartAsync((int)Math.Ceiling(iterations / (double)workersCount), packageSize);
            }

            Task.WaitAll(workers);

            watch.Stop();
            Console.WriteLine($"Done in {watch.Elapsed.TotalSeconds}s, which is {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"There was {exceptions.Count} exceptions");
            Console.ReadKey();
        }

        private static async Task StartAsync(int iterations, int packageSize)
        {
            string configPath = "KronosConfig.json";
            try
            {
                byte[] package = new byte[packageSize * 1024 * 1024];
                new Random().NextBytes(package);

                for (int i = 0; i < iterations; i++)
                {
                    IKronosClient client = KronosClientFactory.CreateClient(configPath);

                    Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    string key = Guid.NewGuid().ToString();
                    DateTime expiryDate = DateTime.UtcNow.AddSeconds(ExpirySecond);

                    Debug.WriteLine($"ADD - testing");
                    await client.InsertAsync(key, package, expiryDate);
                    Debug.WriteLine($" ADD - done (size: {package.Length})");

                    Debug.WriteLine($" COUNT - testing");
                    int count = await client.CountAsync();
                    Debug.WriteLine($" COUNT - done (count: {count})");

                    Debug.WriteLine($" CONTAINS - testing");
                    bool contains = await client.ContainsAsync(key);
                    Debug.WriteLine($"CONTAINS - done (exists: {contains})");

                    Debug.WriteLine($" GET - testing");
                    byte[] fromServer = await client.GetAsync(key);
                    Debug.WriteLine($" GET - done (size: {fromServer.Length})");

                    if (fromServer.Length != package.Length)
                        throw new Exception(
                            $"Received message is invalid! Size should be {package.Length}, but wit {fromServer.Length}");

                    Debug.WriteLine($" DELETE - testing");
                    await client.DeleteAsync(key);
                    bool containsAfterDeletion = await client.ContainsAsync(key);
                    Debug.WriteLine($" DELETE - done (exists after deletion: {containsAfterDeletion})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                exceptions.Add(ex);
            }
        }
    }
}
