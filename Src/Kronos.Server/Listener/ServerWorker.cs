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
    public class ServerWorker : IServerWorker
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

        public Task StartListeningAsync(CancellationToken token)
        {
            return _server.Start(token);
        }

        private void ServerOnOnNewMessage(object sender, MessageArgs args)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                var type = (RequestType)args.UserToken;
                _logger.Info($"Processing new request with Id: {id}, type: {type}, {args.RequestBytes} bytes");
                Request request = _mapper.ProcessRequest(args.RequestBytes, type);
                request.ProcessAndSendResponse(args.Client, Storage);
                _logger.Info($"Processing {id} finished");
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

        public void Dispose()
        {
            _logger.Info("Disposing server");
            _server.Dispose();
        }
    }
}
