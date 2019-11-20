using System;
using System.Diagnostics;
using EntryPoint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kronos.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigLogger();
            PrintLogo();
            CreateHostBuilder(args).Build().Run();
        }

        private static void ConfigLogger()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var settings = Cli.Parse<SettingsArgs>(args);

                    services.AddSingleton(settings);
                    services.AddHostedService<KronosWorker>();
                });
        }
    }
}
