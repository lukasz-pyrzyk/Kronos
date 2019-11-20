using System;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Microsoft.Extensions.Hosting;

namespace Kronos.Server
{
    internal class KronosWorker : IHostedService
    {
        private readonly Listener _server;

        public KronosWorker(SettingsArgs settings)
        {
            var storage = new InMemoryStorage();
            var requestProcessor = new RequestProcessor(storage);

            _server = new Listener(settings, requestProcessor);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _server.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _server.Dispose();
            return Task.CompletedTask;
        }
    }
}