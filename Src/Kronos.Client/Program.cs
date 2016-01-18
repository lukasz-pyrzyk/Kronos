using System;
using System.IO;
using System.Text;
using System.Xml;
using Kronos.Client.Core;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Kronos.Client
{
    public class Program
    {
        private static ILogger logger;
        public static void LoggerSetup()
        {
            var reader = XmlReader.Create("NLog.config");
            var config = new XmlLoggingConfiguration(reader, null); //filename is not required.
            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger();
        }

        public static void Main(string[] args)
        {
            LoggerSetup();
            logger.Info("Starting program");

            IKronosClient client = KronosClientFactory.CreateClient();
            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes($"content of file");
            DateTime expiryDate = new DateTime();

            client.InsertToServer(key, package, expiryDate);

            logger.Info("Closing program");
        }
    }
}
