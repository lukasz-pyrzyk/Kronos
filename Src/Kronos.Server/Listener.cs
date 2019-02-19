using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core;
using Kronos.Core.Messages;
using Kronos.Core.Processing;
using ZeroLog;

namespace Kronos.Server
{
    public class Listener : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly SocketConnection _socketConnection = new SocketConnection();
        private readonly RequestProcessor _requestProcessor;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        private static readonly ILog Logger = LogManager.GetLogger<Listener>();
        private readonly Auth _auth;

        public Listener(SettingsArgs settings, RequestProcessor requestProcessor)
        {
            _auth = Auth.FromCfg(settings.Login, settings.HashedPassword());
            _listener = new TcpListener(IPAddress.Any, settings.Port);
            _requestProcessor = requestProcessor;

            _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        }

        public void Start()
        {
            Logger.Info("Starting server");
            _listener.Start();
            string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Logger.Info($"Server started on {_listener.LocalEndpoint}. Kronos version {version}");

            CancellationToken token = _cancel.Token;

            _ = Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client = null;
                    try
                    {
                        client = await _listener.AcceptTcpClientAsync();
                        var stream = client.GetStream();
                        _ = ProcessSocketConnection(stream);
                    }
                    catch (ObjectDisposedException)
                    {
                        Logger.Info("TCP listener is disposed");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Exception during accepting new request {ex}");
                    }
                    finally
                    {
                        client?.Dispose();
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public void Stop()
        {
            Logger.Info("Stopping server");
            _cancel.Cancel();

            if (_listener.Server.Connected)
            {
                Logger.Info("Server is connected, shutting down");
                try
                {
                    _listener.Server.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException ex)
                {
                    Logger.Error($"Error on shutting down server socket {ex}");
                }
            }

            _listener.Stop();
            _listener.Server.Dispose();

            Logger.Info("Server is down");
        }

        public void Dispose()
        {
            Logger.Info("Stopping TCP/IP server");
            Stop();
        }

        private async Task ProcessSocketConnection(Stream stream)
        {
            try
            {
                Request request = _socketConnection.ReceiveRequest(stream);

                Logger.Debug($"Processing new request {request.Type}");
                Response response = _requestProcessor.Handle(request, _auth);

                await _socketConnection.Send(response, stream);
                Logger.Debug("Processing finished");
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception on processing: {ex}");
            }
        }
    }
}
