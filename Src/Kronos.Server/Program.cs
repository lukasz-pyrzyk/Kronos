using EntryPoint;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
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
                    services.AddSingleton<IStorage, InMemoryStorage>();
                    services.AddSingleton<RequestProcessor>();
                    services.AddSingleton<Listener>();
                    services.AddHostedService<KronosWorker>();
                });
        }
    }
}
