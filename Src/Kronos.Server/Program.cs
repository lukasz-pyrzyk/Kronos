using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
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
        private static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

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

            Task.WaitAll(new[] { StartAsync(port) }, tokenSource.Token);
        }

        public static Task StartAsync(int port)
        {
            return Task.Run(async () =>
            {
                IExpiryProvider expiryProvider = new StorageExpiryProvider();
                using (IStorage storage = new InMemoryStorage(expiryProvider))
                {
                    IProcessor<MessageArgs> processor = new SocketProcessor();
                    IPAddress localAddr = await GetLocalIPAddress();
                    using (IServer server = new XGainServer(localAddr, port, processor))
                    {
                        IRequestMapper mapper = new RequestMapper();
                        IServerWorker worker = new ServerWorker(mapper, storage, server);
                        await worker.StartListeningAsync(tokenSource.Token);
                    }
                }
            });
        }

        public static async Task<IPAddress> GetLocalIPAddress()
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
