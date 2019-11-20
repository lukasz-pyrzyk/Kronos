using EntryPoint;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
using Kronos.Server.Storage.Cleaning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kronos.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var settings = Cli.Parse<SettingsArgs>(args);

                    services.AddSingleton(settings);
                    services.AddSingleton<ICleaner, Cleaner>();
                    services.AddSingleton<IScheduler, Scheduler>();
                    services.AddSingleton<InMemoryStorage>();
                    services.AddSingleton<RequestProcessor>();
                    services.AddSingleton<Listener>();
                    services.AddHostedService<KronosWorker>();
                });
        }
    }
}
