using System;
using System.Net;
using System.Text;
using Kronos.Client;

namespace ClientSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host = "23.101.135.101"; // IP of the Kronos.Server node
            int port = 5001;  // Opened port in the node

            IKronosClient client = KronosClientFactory.CreateClient(IPAddress.Parse(host), port);

            string key = Guid.NewGuid().ToString();
            byte[] package = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiryDate = DateTime.Now.AddDays(1);

            client.InsertToServer(key, package, expiryDate);

            byte[] fromServer = client.TryGetValue(key);

            Console.WriteLine(Encoding.UTF8.GetString(fromServer));
            Console.ReadKey();
        }
    }
}
