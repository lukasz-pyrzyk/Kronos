using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;

namespace Benchmark.Config
{
    public class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            Add(StatisticColumn.Mean);
            Add(StatisticColumn.Median);
            Add(CsvMeasurementsExporter.Default);
            Add(RPlotExporter.Default);
            Add(MemoryDiagnoser.Default);
        }
    }
}
