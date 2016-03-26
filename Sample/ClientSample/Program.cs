using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Client;

namespace ClientSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string configPath = "KronosConfig.json";

            IKronosClient client = KronosClientFactory.CreateClient(configPath);

            var watch = Stopwatch.StartNew();
            byte[] package = File.ReadAllBytes(@"C:\Users\lpyrz_000\Source\Repos\Kronos\Sample\ClientSample\project.lock.json");

            List<Task> workers = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                Task worker = Task.Run(() =>
                {
                    string key = Guid.NewGuid().ToString();
                    DateTime expiryDate = DateTime.Now.AddDays(1);

                    client.Insert(key, package, expiryDate);
                    byte[] fromServer = client.Get(key);

                    string deserialized = Encoding.UTF8.GetString(fromServer);
                    Console.WriteLine($"{deserialized.Length} - {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                });

                workers.Add(worker);

                if (workers.Count > Environment.ProcessorCount) Task.WaitAny(workers.ToArray());
            }
            Task.WaitAll(workers.ToArray());
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
}
