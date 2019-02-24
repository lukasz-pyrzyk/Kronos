using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClientSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.WaitAll(StartAsync());
        }

        private static async Task StartAsync()
        {
            IKronosClient client = KronosClientFactory.FromIp("127.0.0.1", 44000);

            var watch = Stopwatch.StartNew();
            byte[] package = new byte[1024 * 9];
            new Random().NextBytes(package);

            for (int i = 0; i < 10000; i++)
            {
                Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                string key = Guid.NewGuid().ToString();
                DateTimeOffset expiryDate = DateTimeOffset.UtcNow.AddDays(1);

                Debug.WriteLine("ADD - testing");
                await client.InsertAsync(key, package, expiryDate);
                Debug.WriteLine($"ADD - done (size: {package.Length})");

                Debug.WriteLine("COUNT - testing");
                var countResponse = await client.CountAsync();
                Debug.WriteLine($"COUNT - done (count: {countResponse.Count})");

                Debug.WriteLine("CONTAINS - testing");
                var containsResponse = await client.ContainsAsync(key);
                Debug.WriteLine($"CONTAINS - done (exists: {containsResponse.Contains})");

                Debug.WriteLine("GET - testing");
                var getResponse = await client.GetAsync(key);
                Debug.WriteLine($"GET - done (size: {getResponse.Data.Length})");
                Debug.Assert(getResponse.Data.Length == package.Length);

                Debug.WriteLine("DELETE - testing");
                await client.DeleteAsync(key);
                var containsAfterDeletionResponse = await client.ContainsAsync(key);
                Debug.WriteLine($"DELETE - done (exists after deletion: {containsAfterDeletionResponse.Contains})");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
