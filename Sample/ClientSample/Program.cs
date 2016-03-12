using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using Kronos.Client;
using Kronos.Core.Serialization;

namespace ClientSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host = "192.168.56.1"; // IP of the Kronos.Server node
            int port = 5000;  // Opened port in the node

            IKronosClient client = KronosClientFactory.CreateClient(IPAddress.Parse(host), port);

            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("elo");
            DateTime expiryDate = DateTime.Now.AddDays(1);

            client.InsertToServer(key, package, expiryDate);

            var watch = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                byte[] fromServer = client.TryGetValue(key);
                Console.WriteLine($"{SerializationUtils.Deserialize<string>(fromServer)} - {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
}
