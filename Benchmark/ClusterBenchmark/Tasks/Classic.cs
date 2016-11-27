using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark.Tasks
{
    public class Classic : Benchmark
    {
        protected override async Task RunInternalAsync(IKronosClient client, byte[] package)
        {
            string key = Guid.NewGuid().ToString();
            await client.InsertAsync(key, package, DateTime.UtcNow.AddDays(5));
            byte[] data = await client.GetAsync(key);
            Debug.Assert(data.SequenceEqual(package));
            await client.DeleteAsync(key);
        }
    }
}
