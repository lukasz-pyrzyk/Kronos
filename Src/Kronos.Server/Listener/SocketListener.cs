using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Kronos.Core.StatusCodes;
using Kronos.Server.RequestProcessing;
using Kronos.Server.Storage;
using NLog;

namespace Kronos.Server.Listener
{
    public class SocketListener : ICommunicationListener
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _processor = new RequestProcessor();

        public const int QueueSize = 5;
        public const int Port = 5000;
        public const int BufferSize = 5555;
        private const int bufferSize = 1024 * 8;

        public void StartListening()
        {
            Socket server = null;
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, Port));
                server.Listen(QueueSize);

                _logger.Info($"Listening on port {Port}, queue size is {QueueSize} and the buffer is {BufferSize}");

                while (true)
                {
                    Socket client = null;
                    try
                    {
                        client = server.Accept();
                        _logger.Info("Accepting new request");
                        var timer = Stopwatch.StartNew();

                        byte[] requestBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[bufferSize];
                            using (NetworkStream stream = new NetworkStream(client))
                            {
                                do
                                {
                                    int received = stream.Read(buffer, 0, buffer.Length);
                                    ms.Write(buffer, 0, received);
                                } while (stream.DataAvailable);
                            }
                            requestBytes = ms.ToArray();
                        }

                        timer.Stop();
                        _logger.Info($"Finished receiving package in {timer.ElapsedMilliseconds}ms");
                        
                        _logger.Info("Processing request");
                        _processor.ProcessRequest(client, requestBytes);
                    }
                    catch (SocketException ex)
                    {
                        _logger.Error(
                            $"Exception during receiving request from client {client?.RemoteEndPoint}");
                        _logger.Fatal(ex);
                    }
                    finally
                    {
                        try
                        {
                            client?.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException)
                        {
                        }
                        try
                        {
                            client?.Dispose();
                        }
                        catch (SocketException)
                        {
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                _logger.Fatal(ex);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
            finally
            {
                _logger.Info("Disposing server");
                server?.Shutdown(SocketShutdown.Both);
                server?.Dispose();
            }
        }

        public void Dispose()
        {
            InMemoryStorage.Clear();
        }
    }
}
