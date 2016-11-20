using System;
using System.Diagnostics;
using System.IO;
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
            string configPath = "KronosConfig.json";

            IKronosClient client = KronosClientFactory.CreateClient(configPath);

            var watch = Stopwatch.StartNew();
            byte[] package = new byte[1024 * 9];
            new Random().NextBytes(package);

            for (int i = 0; i < 10000; i++)
            {
                Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                string key = Guid.NewGuid().ToString();
                DateTime expiryDate = DateTime.UtcNow.AddDays(1);

                Debug.WriteLine("ADD - testing");
                await client.InsertAsync(key, package, expiryDate);
                Debug.WriteLine($"ADD - done (size: {package.Length})");

                Debug.WriteLine("COUNT - testing");
                int count = await client.CountAsync();
                Debug.WriteLine($"COUNT - done (count: {count})");

                Debug.WriteLine("CONTAINS - testing");
                bool contains = await client.ContainsAsync(key);
                Debug.WriteLine($"CONTAINS - done (exists: {contains})");

                Debug.WriteLine("GET - testing");
                byte[] fromServer = await client.GetAsync(key);
                Debug.WriteLine($"GET - done (size: {fromServer.Length})");

                Debug.WriteLine("DELETE - testing");
                await client.DeleteAsync(key);
                bool containsAfterDeletion = await client.ContainsAsync(key);
                Debug.WriteLine($"DELETE - done (exists after deletion: {containsAfterDeletion})");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        private static async Task SendFileFilesAsync()
        {
            const string directory = @"path";
            string configPath = "KronosConfig.json";

            IKronosClient client = KronosClientFactory.CreateClient(configPath);

            foreach (string file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                var time = Stopwatch.StartNew();
                var data = File.ReadAllBytes(file);
                string name = Path.GetFileName(file);
                Console.WriteLine($"Sending {name}");
                await client.InsertAsync(name, data, DateTime.Now.AddMinutes(10));
                time.Stop();
                Console.WriteLine($"Sending finished. Time: {time.ElapsedMilliseconds}");
            }
        }
    }
}
