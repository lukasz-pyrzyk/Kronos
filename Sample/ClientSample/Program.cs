using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Kronos.Client;
using Kronos.Core.Serialization;

namespace ClientSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string configPath = "KronosConfig.json";

            IKronosClient client = KronosClientFactory.CreateClient(configPath);

            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("elo");
            DateTime expiryDate = DateTime.Now.AddDays(1);

            client.InsertToServer(key, package, expiryDate);

            var watch = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                byte[] fromServer = client.TryGetValue(key);
                string deserialized = Encoding.UTF8.GetString(fromServer);
                Console.WriteLine($"{deserialized} - {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
}
