using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Kronos.Core.Communication;
using Kronos.Core.Processors;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using NLog;
using NLog.Config;
using XGain;
using XGain.Processing;

namespace Kronos.Server
{
    public class Program
    {
        public static void LoggerSetup()
        {
            var reader = XmlReader.Create("NLog.config");
            var config = new XmlLoggingConfiguration(reader, null); //filename is not required.
            LogManager.Configuration = config;
        }

        public static void Main(string[] args)
        {
            LoggerSetup();

            int port = Convert.ToInt32(args[0]);

            Task.WaitAll(StartAsync(port));
        }

        public static async Task StartAsync(int port)
        {
            IPAddress localAddr = await GetLocalIPAddress();

            IProcessor<MessageArgs> processor = new SocketProcessor();

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            IStorage storage = new InMemoryStorage(expiryProvider);

            IServer server = new XGainServer(localAddr, port, processor);
            IRequestProcessor mapper = new RequestProcessor(storage);
            IServerWorker worker = new ServerWorker(mapper, storage, server);
            worker.Start();

            var reset = new ManualResetEventSlim();
            Console.CancelKeyPress += (sender, e) =>
            {
                reset.Set();
            };

            reset.Wait();

            // dispose components
            storage.Dispose();
            server.Dispose();
            worker.Dispose();
        }

        private static async Task<IPAddress> GetLocalIPAddress()
        {
            var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception("Local IP Address Not Found!");
        }
    }
}
