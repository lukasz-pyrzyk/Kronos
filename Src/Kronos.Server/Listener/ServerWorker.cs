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
        }

        public void StartListening()
        {
            _logger.Info("Starting server");

            _server.OnNewMessage += (sender, message) =>
            {
                _processor.ProcessRequest(message.Client, message.RequestBytes, (RequestType)message.UserToken, Storage);
            };

            Task serverTask = _server.Start();
            serverTask.Wait();

            _logger.Info("Shutting down server");
        }


        public void Dispose()
        {
            Storage.Clear();
            _server.Dispose();
        }
    }
}
