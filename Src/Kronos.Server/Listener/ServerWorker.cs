using System;
using System.Buffers;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Kronos.Server.EventArgs;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;

namespace Kronos.Server.Listener
{
    public class ServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _requestsProcessor;
        private readonly IServer _server;
        public IStorage Storage { get; }

        public ServerWorker(IRequestProcessor requestsProcessor, IStorage storage, IServer server)
        {
            _requestsProcessor = requestsProcessor;
            _server = server;
            Storage = storage;

            _server.OnNewMessage += OnMessage;
            _server.OnStart += OnStart;
            _server.OnError += OnError;
        }

        public void Start()
        {
            _server.Start();
        }

        private void OnMessage(object sender, MessageArgs message)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                var args = message as ReceivedMessage;
                _logger.Debug($"Processing new request with Id: {id}, type: {args.Type}, {args.Received} bytes");
                _requestsProcessor.HandleIncomingRequest(args.Type, args.RequestBytes, args.Received, args.Client);
                _logger.Debug($"Processing {id} finished");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(message.RequestBytes);
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
