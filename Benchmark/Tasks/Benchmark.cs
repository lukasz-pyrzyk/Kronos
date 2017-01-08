using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark.Tasks
{
    public abstract class Benchmark
    {
        public async Task<Results> Run(string configPath, int iterations, int workersCount, int packageSize)
        {
            var results = new Results();
            var watch = new Stopwatch();
            try
            {
                IKronosClient client = KronosClientFactory.CreateClient(configPath);
                await WarnupServer(client);

                var package = PrepareData(Mb(packageSize));

                int iterationsPerWorker = (int)Math.Ceiling(iterations / (double)workersCount);
                Console.WriteLine($"Starting {workersCount} workers with {iterationsPerWorker} iterations per each one");
                Task[] workers = new Task[workersCount];

                watch.Start();
                for (int w = 0; w < workersCount; w++)
                {
                    Task workerTask = Task.Run(async () => {
                        IKronosClient ownClient = KronosClientFactory.CreateClient(configPath);
                        for (int i = 0; i < iterationsPerWorker; i++)
                        {
                            try
                            {
                                await RunInternalAsync(ownClient, package);
                            }
                            catch(Exception ex)
                            {
                                results.Exceptions.Add(ex);
                            }
                        }
                    });

                    workers[w] = workerTask;
                }

                await Task.WhenAll(workers);

                watch.Stop();
                results.Time = watch.Elapsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                watch.Stop();
                results.Time = watch.Elapsed;
                results.Exceptions.Add(ex);
            }

            return results;
        }

        private async Task WarnupServer(IKronosClient client)
        {
            Console.WriteLine("Warnup");
            var watch = Stopwatch.StartNew();
            await SendToWarnup(client, size: 10, times: 10000);
            await SendToWarnup(client, size: Mb(5), times: 10);
            watch.Stop();
            Console.WriteLine($"Warnup finished in {watch.ElapsedMilliseconds}ms");
        }

        private async Task SendToWarnup(IKronosClient client, int size, int times)
        {
            byte[] package = PrepareData(size);
            for (int i = 0; i < times; i++)
            {
                string key = Guid.NewGuid().ToString();
                await client.InsertAsync(key, package, DateTime.UtcNow.AddDays(1));
            }
        }

        protected virtual byte[] PrepareData(int packageSize)
        {
            byte[] package = new byte[packageSize];
            new Random().NextBytes(package);
            return package;
        }

        private static int Mb(int bytes) => bytes*1024*1024;

        protected abstract Task RunInternalAsync(IKronosClient client, byte[] package);
    }
}
