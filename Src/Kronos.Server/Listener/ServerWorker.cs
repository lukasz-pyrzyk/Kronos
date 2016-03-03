using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Kronos.Server.RequestProcessing;
using Kronos.Server.Storage;
using NLog;

namespace Kronos.Server.Listener
{
    internal class SocketServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _processor;
        private readonly IStorage _storage;

        private const int bufferSize = 1024 * 8;

        internal SocketServerWorker(IRequestProcessor processor, IStorage storage)
        {
            _processor = processor;
            _storage = storage;
        }

        public SocketServerWorker() : this(new RequestProcessor(), new InMemoryStorage())
        {
        }

        public void StartListening(Socket server)
        {
            try
            {
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
                        _processor.ProcessRequest(client, requestBytes, _storage);
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
            _storage.Clear();
        }
    }
}
