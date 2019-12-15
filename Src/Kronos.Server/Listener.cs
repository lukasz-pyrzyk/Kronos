using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core;
using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Microsoft.Extensions.Logging;

namespace Kronos.Server
{
    class Listener
    {
        private readonly TcpListener _tcpListener;
        private readonly SocketConnection _socketConnection = new SocketConnection();
        private readonly RequestProcessor _requestProcessor;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        private readonly Auth _auth;
        private readonly ILogger<Listener> _logger;

        public Listener(SettingsArgs settings, RequestProcessor requestProcessor, ILogger<Listener> logger)
        {
            _auth = Auth.FromCfg(settings.Login, settings.HashedPassword());
            _tcpListener = new TcpListener(IPAddress.Any, settings.Port);
            _requestProcessor = requestProcessor;
            _logger = logger;

            _tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        }

        public void Start()
        {
            _logger.LogInformation("Starting server");
            _tcpListener.Start();
            string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            _logger.LogInformation("Server started on {endpoint}. Kronos version {appVersion}", _tcpListener.LocalEndpoint, version);

            CancellationToken token = _cancel.Token;
            _ = Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    _ = Task.Run(() => ProcessConnection(client), token);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public void Stop()
        {
            _logger.LogInformation("Stopping server");
            _cancel.Cancel();

            if (_tcpListener.Server.Connected)
            {
                _logger.LogInformation("Server is connected, shutting down");
                try
                {
                    _tcpListener.Server.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException ex)
                {
                    _logger.LogError("Error on shutting down server socket {ex}", ex);
                }
            }

            _tcpListener.Stop();
            _tcpListener.Server.Dispose();

            _logger.LogInformation("Server is down");
        }

        private async Task ProcessConnection(TcpClient client)
        {
            try
            {
                using var connection = client;
                await using var stream = connection.GetStream();

                Request request = _socketConnection.ReceiveRequest(stream);

                _logger.LogDebug("Processing new request {requestType}", request.Type);
                Response response = _requestProcessor.Handle(request, _auth);

                await _socketConnection.Send(response, stream);
                _logger.LogDebug("Request {requestType} finished", request.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception on processing: {ex}", ex);
            }
        }
    }
}
