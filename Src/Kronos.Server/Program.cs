using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Kronos.Server.Listening;
using NLog;
using NLog.Config;

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

            PrintLogo();

            int port = 5000;
            if (args.Length == 1)
            {
                int.TryParse(args[0], out port);
            }

            Task.WaitAll(StartAsync(port));
        }

        public static async Task StartAsync(int port)
        {
            IPAddress localAddr = await EndpointUtils.GetIPAsync();

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            IStorage storage = new InMemoryStorage(expiryProvider);

            IRequestProcessor requestProcessor = new RequestProcessor(storage);
            IProcessor processor = new SocketProcessor();
            IListener server = new Listener(localAddr, port, processor, requestProcessor);

            server.Start();

            var reset = new ManualResetEventSlim();
            Console.CancelKeyPress += (sender, e) =>
            {
                reset.Set();
            };

            reset.Wait();

            // dispose components
            storage.Dispose();
            server.Dispose();
        }

        private static void PrintLogo()
        {
            PrintLogoLine("");
            PrintLogoLine("  _  __  _____     ____    _   _    ____     _____ ");
            PrintLogoLine(@" | |/ / |  __ \   / __ \  | \ | |  / __ \   / ____|");
            PrintLogoLine(@" | ' /  | |__) | | |  | | |  \| | | |  | | | (___  ");
            PrintLogoLine(@" |  <   |  _  /  | |  | | | . ` | | |  | |  \___ \ ");
            PrintLogoLine(@" | . \  | | \ \  | |__| | | |\  | | |__| |  ____) |");
            PrintLogoLine(@" |_|\_\ |_|  \_\  \____/  |_| \_|  \____/  |_____/ ");
            PrintLogoLine("");
            PrintLogoLine("");
            PrintLogoLine("");
        }

        private static void PrintLogoLine(string line)
        {
            int centerPosition = (Console.WindowWidth - line.Length) / 2;
            if(centerPosition > 0) // if it's Docker console, it might be less than zero
            {
                Console.SetCursorPosition(centerPosition, Console.CursorTop);
            }

            Console.WriteLine(line);
        }
    }
}
