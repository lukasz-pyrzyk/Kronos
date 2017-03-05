using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using EntryPoint;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NLog;
using NLog.Config;

namespace Kronos.Server
{
    public class Program
    {
        public static bool IsWorking { get; private set; }

        private static readonly ManualResetEventSlim _cancelEvent = new ManualResetEventSlim();

        public static void Main(string[] args)
        {
            var settings = Cli.Parse<SettingsArgs>(args);

            PrintLogo();

            var config = LoggerSetup();
            Task.WaitAll(StartAsync(settings, config));
        }

        public static async Task StartAsync(SettingsArgs settings, LoggingConfiguration config)
        {
            LogManager.Configuration = config;

            IPAddress localAddr = await EndpointUtils.GetIPAsync();

            IStorage storage = new InMemoryStorage();

            IRequestProcessor requestProcessor = new RequestProcessor(storage);
            ISocketProcessor processor = new SocketProcessor();
            IListener server = new Listener(localAddr, settings, processor, requestProcessor);

            server.Start();
            IsWorking = true;

            Console.CancelKeyPress += (sender, args) => Stop();

            _cancelEvent.Wait();
            _cancelEvent.Reset();

            // dispose components
            storage.Dispose();
            server.Dispose();
        }

        public static void Stop()
        {
            _cancelEvent.Set();
            IsWorking = false;
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
            if (centerPosition > 0) // if it's Docker console, it might be less than zero
            {
                Console.SetCursorPosition(centerPosition, Console.CursorTop);
            }

            Console.WriteLine(line);
        }

        private static XmlLoggingConfiguration LoggerSetup()
        {
            var reader = XmlReader.Create("NLog.config");
            return new XmlLoggingConfiguration(reader, null);
        }
    }
}
