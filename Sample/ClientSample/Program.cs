using System;
using System.IO;
using System.Net;
using Kronos.Client;

namespace ClientSample
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Starting program");

            IPAddress host = IPAddress.Parse("192.168.0.10");
            int port = 5000;
            IKronosClient client = KronosClientFactory.CreateClient(host, port);

            string key = "key";
            string fileToSend = "C:\\Temp\\file.bin";
            DateTime expiryDate = new DateTime();

            client.InsertToServer(key, File.ReadAllBytes(fileToSend), expiryDate);

            Console.WriteLine("Closing program");
        }
    }
}
