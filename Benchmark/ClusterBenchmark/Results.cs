using System;
using System.Collections.Generic;

namespace ClusterBenchmark
{
    public class Results
    {
        public List<Exception> Exceptions { get; set; } = new List<Exception>();
        public TimeSpan Time { get; set; }
    }
}
