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

            Console.ReadKey();
        }

        private static async Task StartAsync()
        {
            string configPath = "KronosConfig.json";

            IKronosClient client = KronosClientFactory.CreateClient(configPath);

            var watch = Stopwatch.StartNew();
            byte[] package = new byte[1024 * 9];
            new Random().NextBytes(package);

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                string key = Guid.NewGuid().ToString();
                DateTime expiryDate = DateTime.UtcNow.AddDays(1);

                Console.WriteLine("ADD - testing");
                await client.InsertAsync(key, package, expiryDate);
                Console.WriteLine($"ADD - done (size: {package.Length})");

                Console.WriteLine("COUNT - testing");
                int count = await client.CountAsync();
                Console.WriteLine($"COUNT - done (count: {count})");

                Console.WriteLine("CONTAINS - testing");
                bool contains = await client.ContainsAsync(key);
                Console.WriteLine($"CONTAINS - done (exists: {contains})");

                Console.WriteLine("GET - testing");
                byte[] fromServer = await client.GetAsync(key);
                Console.WriteLine($"GET - done (size: {fromServer.Length})");

                Console.WriteLine("GET - testing");
                await client.DeleteAsync(key);
                bool containsAfterDeletion = await client.ContainsAsync(key);
                Console.WriteLine($"GET - done (exists after deletion: {containsAfterDeletion})");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
