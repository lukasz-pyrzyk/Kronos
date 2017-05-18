using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;

namespace Benchmark.Config
{
    [OrderProvider(SummaryOrderPolicy.FastestToSlowest)]
    public class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            Add(StatisticColumn.Mean);
            Add(StatisticColumn.StdErr);
            Add(StatisticColumn.StdDev);
            Add(StatisticColumn.Median);
            Add(CsvMeasurementsExporter.Default);
            Add(RPlotExporter.Default);
            Add(MemoryDiagnoser.Default);
        }
    }
}
