using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
using Kronos.Core.Processing;
using NLog;

namespace Kronos.Server
{
    public class Listener : IListener
    {
        private readonly TcpListener _listener;
        private readonly ISocketProcessor _processor;
        private readonly IRequestProcessor _requestProcessor;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Auth _auth;

        public Listener(SettingsArgs settings, ISocketProcessor processor, IRequestProcessor requestProcessor)
        {
            _auth = Auth.FromCfg(new AuthConfig { Login = settings.Login, Password = settings.Password });
            _listener = new TcpListener(IPAddress.Any, settings.Port);
            _processor = processor;
            _requestProcessor = requestProcessor;
        }

        public void Start()
        {
            _logger.Info("Starting server");
            _listener.Start();
            string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            _logger.Info($"Server started on {_listener.LocalEndpoint}. Kronos version {version}");

            CancellationToken token = _cancel.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    Socket socket = null;
                    try
                    {
                        socket = await _listener.AcceptSocketAsync().ConfigureAwait(false);
                        ProcessSocketConnection(socket);
                    }
                    catch (ObjectDisposedException)
                    {
                        _logger.Info("TCP listener is disposed");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Exception during accepting new request {ex}");
                    }
                    finally
                    {
                        socket?.Shutdown(SocketShutdown.Send);
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

        private void ProcessSocketConnection(Socket client)
        {
            string id = Guid.NewGuid().ToString();
            client.SendTimeout = 10000;
            client.ReceiveTimeout = 10000;
            try
            {
                Request request = _processor.ReceiveRequest(client);

                _logger.Debug($"Processing new request {request.Type} with Id: {id}");
                Response response = _requestProcessor.Handle(request, _auth);

                _processor.SendResponse(client, response);
                _logger.Debug($"Processing {id} finished");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception on processing request {id}, {ex}");
            }
        }
    }
}
