using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core;
using Kronos.Core.Messages;
using Kronos.Core.Processing;

namespace Kronos.Server
{
    public class Listener : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly SocketConnection _socketConnection = new SocketConnection();
        private readonly RequestProcessor _requestProcessor;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

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
            Trace.TraceInformation("Starting server");
            _listener.Start();
            string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Trace.TraceInformation($"Server started on {_listener.LocalEndpoint}. Kronos version {version}");

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
                        Trace.TraceInformation("TCP listener is disposed");
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"Exception during accepting new request {ex}");
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
            Trace.TraceInformation("Stopping server");
            _cancel.Cancel();

            if (_listener.Server.Connected)
            {
                Trace.TraceInformation("Server is connected, shutting down");
                try
                {
                    _listener.Server.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException ex)
                {
                    Trace.TraceError($"Error on shutting down server socket {ex}");
                }
            }

            _listener.Stop();
            _listener.Server.Dispose();

            Trace.TraceInformation("Server is down");
        }

        public void Dispose()
        {
            Trace.TraceInformation("Stopping TCP/IP server");
            Stop();
        }

        private async Task ProcessSocketConnection(Stream stream)
        {
            try
            {
                Request request = _socketConnection.ReceiveRequest(stream);

                Trace.TraceInformation($"Processing new request {request.Type}");
                Response response = _requestProcessor.Handle(request, _auth);

                await _socketConnection.Send(response, stream);
                Trace.TraceInformation("Processing finished");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception on processing: {ex}");
            }
        }
    }
}
