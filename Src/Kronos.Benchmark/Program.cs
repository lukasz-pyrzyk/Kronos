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
            BenchmarkRunner.Run<InMemoryStorageBenchmark>(DefaultConfig.Instance.RemoveBenchmarkFiles());
        }
    }
}
