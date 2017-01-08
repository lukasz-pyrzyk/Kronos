using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClusterBenchmark.Tasks
{
    public class All : Benchmark
    {
        protected override async Task RunInternalAsync(IKronosClient client, byte[] package)
        {
            string key = Guid.NewGuid().ToString();
            DateTime expiryDate = DateTime.UtcNow.AddSeconds(5);

            Debug.WriteLine($"ADD - testing");
            await client.InsertAsync(key, package, expiryDate);
            Debug.WriteLine($" ADD - done (size: {package.Length})");

            Debug.WriteLine($" COUNT - testing");
            int count = await client.CountAsync();
            Debug.WriteLine($" COUNT - done (count: {count})");

            Debug.WriteLine($" CONTAINS - testing");
            bool contains = await client.ContainsAsync(key);
            Debug.WriteLine($"CONTAINS - done (exists: {contains})");

            Debug.WriteLine($" GET - testing");
            byte[] fromServer = await client.GetAsync(key);
            Debug.WriteLine($" GET - done (size: {fromServer.Length})");

            if (fromServer.Length != package.Length)
                throw new Exception(
                    $"Received message is invalid! Size should be {package.Length}, but wit {fromServer.Length}");

            Debug.WriteLine($" DELETE - testing");
            await client.DeleteAsync(key);
            bool containsAfterDeletion = await client.ContainsAsync(key);
            Debug.WriteLine($" DELETE - done (exists after deletion: {containsAfterDeletion})");
        }
    }
}
