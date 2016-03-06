using System.Xml;
using Kronos.Server.Listener;
using NLog;
using NLog.Config;

namespace Kronos.Server
{
    public class Program
    {
        private static ILogger _logger;
        private static TcpServer _server = new TcpServer(new ServerWorker());

        public static void LoggerSetup()
        {
            var reader = XmlReader.Create("NLog.config");
            var config = new XmlLoggingConfiguration(reader, null); //filename is not required.
            LogManager.Configuration = config;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public static void Main()
        {
            LoggerSetup();

            _logger.Info("Starting server");
            _server.Start();
            _logger.Info("Stopping server");
        }
    }
}
