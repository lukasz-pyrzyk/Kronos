using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClusterBenchmark.Tasks;

namespace ClusterBenchmark
{
    public class Program
    {
        private static readonly ConcurrentBag<Exception> Exceptions = new ConcurrentBag<Exception>();
        private static readonly Dictionary<string, Func<Benchmark>> Benchmarks;

        static Program()
        {
            Benchmarks = new Dictionary<string, Func<Benchmark>>
            {
                ["all"] = () => new All(),
                ["classic"] = () => new Classic()
            };
        }

        public static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Passed invalid number arguments");
            }

            string benchmarkName = args[0];
            int iterations = int.Parse(args[1]);
            int packageSize = int.Parse(args[2]);
            bool parallelRun = bool.Parse(args[3]);
            bool localRun = bool.Parse(args[4]);

            Console.WriteLine($"{DateTime.Now:O} Starting benchmark { benchmarkName} with {iterations} iterations, {packageSize}mb data, parallel: {parallelRun}, local: {localRun}");

            int workersCount = parallelRun == false ? 1 : 2;
            Task<Results>[] workers = new Task<Results>[workersCount];

            string config = localRun ? "local.json" : "KronosConfig.json";
            Func<Benchmark> newBenchmark;
            Benchmarks.TryGetValue(benchmarkName, out newBenchmark);
            if (newBenchmark == null)
            {
                Console.WriteLine($"Cannot find benchmark {benchmarkName}");
            }

            for (int i = 0; i < workersCount; i++)
            {
                workers[i] = newBenchmark().Run(config, (int)Math.Ceiling(iterations / (double)workersCount), packageSize);
            }

            Task.WaitAll(workers);

            double time = workers.Max(x => x.Result.Time.TotalMilliseconds);
            Console.WriteLine($"Done in {time}ms, which is {time * 0.001}s");
            Console.WriteLine($"There was {Exceptions.Count} exceptions");
        }
    }
}
