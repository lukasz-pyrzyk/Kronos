using System;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Server.Storage;
using Microsoft.Extensions.Hosting;

namespace Kronos.Server.Workers
{
    class CleanerWorker : IHostedService
    {
        private readonly InMemoryStorage _storage;
        private Timer? _timer;

        public CleanerWorker(InMemoryStorage storage)
        {
            _storage = storage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Clear, null, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(DefaultSettings.CleanupTimeMs));
            return Task.CompletedTask;
        }

        public void Clear(object state)
        {
            _storage.Cleanup();
        }
        
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
            _timer = null;
        }
    }
}
