using System;
using System.Net;
using System.Xml;
using Kronos.Core.Communication;
using Kronos.Core.RequestProcessing;
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
        private static readonly Func<IProcessor<MessageArgs>> ProcessorResolver = () => new SocketProcessor();

        public static void LoggerSetup()
        {
            var reader = XmlReader.Create("NLog.config");
            var config = new XmlLoggingConfiguration(reader, null); //filename is not required.
            LogManager.Configuration = config;
        }

        public static void Main(string[] args)
        {
            int port = Convert.ToInt32(args[0]);

            LoggerSetup();

            IRequestProcessor processor = new RequestProcessor();
            IStorage storage = new InMemoryStorage();
            IServer server = new XGainServer(IPAddress.Any, port, ProcessorResolver);
            IServerWorker worker = new ServerWorker(processor, storage, server);

            worker.StartListening();
        }
    }
}
