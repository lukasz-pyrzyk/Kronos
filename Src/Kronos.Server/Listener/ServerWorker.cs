using System;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using NLog;
using XGain;

namespace Kronos.Server.Listener
{
    internal class ServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestMapper _mapper;
        private readonly IServer _server;
        public IStorage Storage { get; }

        public ServerWorker(IRequestMapper mapper, IStorage storage, IServer server)
        {
            _mapper = mapper;
            _server = server;
            Storage = storage;

            _server.OnNewMessage += ServerOnOnNewMessage;
            _server.OnStart += ServerOnOnStart;
            _server.OnError += ServerOnOnError;
        }

        public void StartListening()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task serverTask = _server.Start(tokenSource.Token);
            serverTask.Wait(tokenSource.Token);

            _logger.Info("Shutting down server");
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        private void ServerOnOnNewMessage(object sender, MessageArgs args)
        {
            try
            {
                Request request = _mapper.ProcessRequest(args.RequestBytes, (RequestType)args.UserToken);
                request.ProcessAndSendResponse(args.Client, Storage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

        }

        private void ServerOnOnStart(object sender, StartArgs args)
        {
            _logger.Info($"Kronos server has been started on {args.LocalEndpoint}");
        }

        private void ServerOnOnError(object sender, ErrorArgs args)
        {
            _logger.Error(args.Exception);
        }
    }
}
