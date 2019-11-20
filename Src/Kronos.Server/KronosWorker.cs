using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kronos.Server
{
    internal class KronosWorker : IHostedService
    {
        private readonly ILogger<KronosWorker> _logger;
        private readonly Listener listener;

        public KronosWorker(Listener listener, ILogger<KronosWorker> logger)
        {
            _logger = logger;
            this.listener = listener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            listener.Start();
            _logger.LogInformation("Worker started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            listener.Stop();
            _logger.LogInformation("Worker stopped");
            return Task.CompletedTask;
        }
    }
}