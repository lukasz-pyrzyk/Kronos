using System;
using System.Threading;
using System.Xml;
using EntryPoint;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NLog;
using NLog.Config;

namespace Kronos.Server
{
    public class Program
    {
        public static bool IsWorking { get; private set; }

        private static readonly ManualResetEventSlim CancelEvent = new ManualResetEventSlim();

        public static void Main(string[] args)
        {
            var settings = Cli.Parse<SettingsArgs>(args);

            PrintLogo();

            var config = LoggerSetup();
            Start(settings, config);
        }

        public static void Start(SettingsArgs settings, LoggingConfiguration config)
        {
            LogManager.Configuration = config;


            IStorage storage = new InMemoryStorage();

            IRequestProcessor requestProcessor = new RequestProcessor(storage);
            var processor = new SocketProcessor();
            IListener server = new Listener(settings, processor, requestProcessor);

            server.Start();
            IsWorking = true;

            Console.CancelKeyPress += (sender, args) => Stop();

            CancelEvent.Wait();
            CancelEvent.Reset();

            // dispose components
            storage.Dispose();
            server.Dispose();
        }

        public static void Stop()
        {
            CancelEvent.Set();
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
