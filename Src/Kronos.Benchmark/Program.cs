using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;

namespace Kronos.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark_StoragePerformance>(DefaultConfig.Instance.RemoveBenchmarkFiles());
        }
    }

    public class Benchmark_StoragePerformance
    {
    }
}
