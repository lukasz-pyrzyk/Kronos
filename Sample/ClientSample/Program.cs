using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
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

            for (int i = 0; i < 10000; i++)
            {
                string key = Guid.NewGuid().ToString();
                byte[] package = Encoding.UTF8.GetBytes("lorem ipsum");
                DateTime expiryDate = DateTime.Now.AddDays(1);

                client.Insert(key, package, expiryDate);
                byte[] fromServer = client.Get(key);

                string deserialized = Encoding.UTF8.GetString(fromServer);
                Console.WriteLine($"{deserialized} - {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
}
