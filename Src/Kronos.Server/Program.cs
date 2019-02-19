using System;
using System.Diagnostics;
using System.Threading;
using EntryPoint;
using Kronos.Core.Processing;
using Kronos.Core.Storage;

namespace Kronos.Server
{
    public class Program
    {
        public static bool IsWorking { get; private set; }

        private static readonly ManualResetEventSlim CancelEvent = new ManualResetEventSlim();

        public static void Main(string[] args)
        {
            var settings = Cli.Parse<SettingsArgs>(args);

            ConfigLogger();
            PrintLogo();

            Start(settings);
        }

        private static void ConfigLogger()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        public static void Start(SettingsArgs settings)
        {
            IStorage storage = new InMemoryStorage();

            var requestProcessor = new RequestProcessor(storage);
            Listener server = new Listener(settings, requestProcessor);

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
    }


    /// <summary>
    /// Comes from https://github.com/dotnet/corefx/pull/32620/files
    /// Should be shipped in .NET Core 3.0
    /// </summary>
    internal class ConsoleTraceListener : TextWriterTraceListener
    {
        public ConsoleTraceListener() : base(Console.Out)
        {
        }

        public ConsoleTraceListener(bool useErrorStream) 
            : base (useErrorStream ? Console.Error : Console.Out)
        {
        }

        public override void Close() { }
    }
}
