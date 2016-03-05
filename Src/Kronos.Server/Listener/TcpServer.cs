using System;
using System.Net;
using System.Net.Sockets;
using Kronos.Core.Communication;
using NLog;

namespace Kronos.Server.Listener
{
    public class TcpServer : IDisposable
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServerWorker _worker;
        private ISocket _server;
        private bool _disposed;

        private const int QueueSize = 1000;
        private const int BufferSize = 65535;

        public bool IsDisposed => _disposed;

        public TcpServer(IServerWorker worker, int port = 5000)
        {
            _server = new KronosSocket(AddressFamily.InterNetwork);

            _server.Bind(new IPEndPoint(IPAddress.Any, port));
            _worker = worker;
        }

        public void Start()
        {
            _server.Listen(QueueSize);
            _logger.Info($"Tcp server is started {_server.LocalEndPoint}, buffer is {BufferSize}");

            _worker.StartListening(_server);
        }

        ~TcpServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        _logger.Trace("Disposing TCP server");
                        if (_server.Connected)
                        {
                            try
                            {
                                _server.Shutdown(SocketShutdown.Both);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        _server.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn($"Exception during server disposing {ex}");
                }
                finally
                {
                    _server = null;
                    _disposed = true;
                }
            }
        }
    }
}
