using System;
using System.Buffers;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Kronos.Server.EventArgs;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;

namespace Kronos.Server.Listening
{
    public class ServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _requestsProcessor;
        private readonly IListener _server;

        public IStorage Storage { get; }

        public ServerWorker(IRequestProcessor requestsProcessor, IStorage storage, IListener server)
        {
            _requestsProcessor = requestsProcessor;
            _server = server;
            Storage = storage;

            _server.OnNewMessage += OnMessageAsync;
            _server.OnStart += OnStart;
            _server.OnError += OnError;
        }

        public void Start()
        {
            _server.Start();
        }

        private async void OnMessageAsync(object sender, RequestArgs request)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                _logger.Debug($"Processing new request with Id: {id}, type: {request.Type}, {request.Received} bytes");
                await _requestsProcessor.HandleAsync(request.Type, request.Request, request.Received, request.Client).ConfigureAwait(false);
                _logger.Debug($"Processing {id} finished");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(request.Request);
            }
        }

        private void OnStart(object sender, StartArgs args)
        {
            string version = PlatformServices.Default.Application.ApplicationVersion;

            _logger.Info($"Kronos server v. {version} has been started on {args.LocalEndpoint} ");
        }

        private void OnError(object sender, ErrorArgs args)
        {
            _logger.Error(args.Exception);
        }

        public void Dispose()
        {
            _logger.Info("Disposing server");
            _server.Dispose();
        }
    }
}
