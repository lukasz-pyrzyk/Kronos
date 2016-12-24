using System;
using System.Collections.Generic;
using ClusterBenchmark.Tasks;
using Benchmark = ClusterBenchmark.Tasks.Benchmark;
using BenchmarkDotNet.Running;
using ClusterBenchmark.Benchmarks;

namespace ClusterBenchmark
{
    public class Program
    {
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
            BenchmarkRunner.Run<AppBenchmark>();

            //if (args.Length != 5)
            //{
            //    Console.WriteLine("Passed invalid number arguments");
            //}

            //string benchmarkName = args[0];
            //int iterations = int.Parse(args[1]);
            //int packageSize = int.Parse(args[2]);
            //bool parallelRun = bool.Parse(args[3]);
            //bool localRun = bool.Parse(args[4]);

            //Console.WriteLine($"{DateTime.Now:O} Starting benchmark { benchmarkName} with {iterations} iterations, {packageSize}mb data, parallel: {parallelRun}, local: {localRun}");

            //int workers = parallelRun == false ? 1 : Environment.ProcessorCount;

            //string config = localRun ? "local.json" : "KronosConfig.json";
            //Func<Benchmark> newBenchmark;
            //Benchmarks.TryGetValue(benchmarkName, out newBenchmark);
            //if (newBenchmark == null)
            //{
            //    Console.WriteLine($"Cannot find benchmark {benchmarkName}");
            //}

            //var result = newBenchmark().Run(config, iterations, workers, packageSize).Result;

            //Console.WriteLine($"Done in {result.Time.TotalMilliseconds}ms, which is {result.Time.TotalSeconds}s, exceptions: {result.Exceptions.Count}");
        }
    }
}
