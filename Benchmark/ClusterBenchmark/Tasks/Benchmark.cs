using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark.Tasks
{
    public abstract class Benchmark
    {
        public async Task<Results> Run(string configPath, int iterations, int packageSize)
        {
            var results = new Results();
            try
            {
                var package = PrepareData(packageSize);

                IKronosClient client = KronosClientFactory.CreateClient(configPath);
                var watch = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    Debug.WriteLine($"Iteration : {i}");
                    await RunInternalAsync(client, package);
                }
                watch.Stop();
                results.Time = watch.Elapsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                results.Exceptions.Add(ex);
            }

            return results;
        }

        protected virtual byte[] PrepareData(int packageSize)
        {
            byte[] package = new byte[packageSize * 1024 * 1024];
            new Random().NextBytes(package);
            return package;
        }

        protected abstract Task RunInternalAsync(IKronosClient client, byte[] package);
    }
}
