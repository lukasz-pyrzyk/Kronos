using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kronos.Server.Workers
{
    internal class ListenerWorker : IHostedService
    {
        private readonly Listener _listener;
        private readonly ILogger<ListenerWorker> _logger;

        public ListenerWorker(Listener listener, ILogger<ListenerWorker> logger)
        {
            _logger = logger;
            _listener = listener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _listener.Start();
            _logger.LogInformation("Worker started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listener.Stop();
            _logger.LogInformation("Worker stopped");
            return Task.CompletedTask;
        }
    }
}