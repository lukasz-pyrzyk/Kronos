using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using NLog;

namespace Kronos.Server.Listening
{
    public class Listener : IListener
    {
        private const int Backlog = 100;

        private readonly TcpListener _listener;
        private readonly IProcessor _processor;
        private readonly IRequestProcessor _requestProcessor;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Listener(IPAddress ipAddress, int port, IProcessor processor, IRequestProcessor requestProcessor)
        {
            _listener = new TcpListener(ipAddress, port);
            _processor = processor;
            _requestProcessor = requestProcessor;
        }

        public void Start()
        {
            _logger.Info("Starting server");
            _listener.Start(Backlog);
            string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            _logger.Info($"Server started on {_listener.LocalEndpoint}. Kronos version {version}");

            CancellationToken token = _cancel.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Socket socket = await _listener.AcceptSocketAsync().ConfigureAwait(false);
                        ProcessSocketConnection(socket);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Exception during accepting new request {ex}");
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public void Stop()
        {
            try
            {
                _cancel.Cancel();
                _listener.Stop();
                _logger.Info("Server has been stopped");
            }
            catch (SocketException ex)
            {
                _logger.Error($"Exception during disposing server: {ex}");
            }
        }

        public void Dispose()
        {
            _logger.Info("Stopping TCP/IP server");
            Stop();
        }

        private void ProcessSocketConnection(Socket socket)
        {
            string id = Guid.NewGuid().ToString();
            RequestArg args = new RequestArg();
            _processor.ReceiveRequest(socket, ref args);
            try
            {
                _logger.Debug($"Processing new request {args.Type} with Id: {id}, {args.Count} bytes");
                byte[] response = _requestProcessor.Handle(args.Type, args.Bytes, args.Count);
                _logger.Debug($"Sending response with {response.Length} bytes to the user");
                SocketUtils.SendAll(socket, response);

                _logger.Debug($"Processing {id} finished");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception on processing request {id}, {ex}");
            }
        }
    }
}
