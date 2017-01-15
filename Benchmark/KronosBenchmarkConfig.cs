using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;

namespace ClusterBenchmark
{
    [OrderProvider(SummaryOrderPolicy.FastestToSlowest)]
    public class KronosBenchmarkConfig : ManualConfig
    {
        public KronosBenchmarkConfig()
        {
            Add(StatisticColumn.Min);
            Add(StatisticColumn.Max);
            Add(MemoryDiagnoser.Default);
            Add(MarkdownExporter.GitHub);
            Add(PlainExporter.Default);
            Add(AsciiDocExporter.Default);
        }
    }
}
