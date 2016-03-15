using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.RequestProcessing;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using NLog;
using XGain;

namespace Kronos.Server.Listener
{
    internal class ServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _processor;
        private readonly IServer _server;
        public IStorage Storage { get; }

        public ServerWorker(IRequestProcessor processor, IStorage storage, IServer server)
        {
            _processor = processor;
            _server = server;
            Storage = storage;

            _server.OnNewMessage += ServerOnOnNewMessage;
            _server.OnStart += ServerOnOnStart;
            _server.OnError += ServerOnOnError;
        }

        public void StartListening()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task serverTask = _server.StartSynchronously(tokenSource.Token);
            serverTask.Wait(tokenSource.Token);

            _logger.Info("Shutting down server");
        }

        public void Dispose()
        {
            Storage.Clear();
            _server.Dispose();
        }

        private void ServerOnOnNewMessage(object sender, MessageArgs args)
        {
            _processor.ProcessRequest(args.Client, args.RequestBytes, (RequestType)args.UserToken, Storage);
        }

        private void ServerOnOnStart(object sender, StartArgs args)
        {
            _logger.Info($"Kronos server has been started on {args.LocalEndpoint} with {args.ProcessingType} mode");
        }

        private void ServerOnOnError(object sender, ErrorArgs args)
        {
            _logger.Error(args.Exception);
        }
    }
}
