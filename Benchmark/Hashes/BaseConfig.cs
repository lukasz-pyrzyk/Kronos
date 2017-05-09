using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;

namespace Benchmark.Hashes
{
    public class BaseConfig : ManualConfig
    {
        public BaseConfig()
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