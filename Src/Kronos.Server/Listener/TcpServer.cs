using System;
using System.Net;
using System.Net.Sockets;
using NLog;

namespace Kronos.Server.Listener
{
    public class TcpServer : IDisposable
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private Socket _server;

        private const int QueueSize = 1000;
        private const int Port = 5000;
        private const int BufferSize = 1024 * 8;

        public TcpServer(IServerWorker worker)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = BufferSize,
                SendBufferSize = BufferSize,
                NoDelay = true // Nagle algorithm
            };

            _server.Bind(new IPEndPoint(IPAddress.Any, Port));
            _server.Listen(QueueSize);
            _logger.Info($"Tcp server is started on port {Port}, buffer is {BufferSize}");

            worker.StartListening(_server);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            try
            {
                _logger.Trace("Disposing TCP server");
                try
                {
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
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"Exception during server disposing {ex}");
            }
            finally
            {
                _server = null;
            }
        }
    }
}
