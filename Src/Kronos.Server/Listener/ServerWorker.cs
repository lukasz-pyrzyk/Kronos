using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using Kronos.Server.RequestProcessing;
using NLog;
using ProtoBuf;

namespace Kronos.Server.Listener
{
    internal class SocketServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _processor;
        public IStorage Storage { get; }

        private const int bufferSize = 1024 * 8;

        internal SocketServerWorker(IRequestProcessor processor, IStorage storage)
        {
            _processor = processor;
            Storage = storage;
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

                        byte[] packageSizeBuffer = new byte[sizeof(int)];
                        _logger.Info("Receiving information about request size");
                        client.Receive(packageSizeBuffer, SocketFlags.None);

                        int requestSize = SerializationUtils.GetLengthOfPackage(packageSizeBuffer);
                        _logger.Info($"Request contains {requestSize} bytes");

                        byte[] r;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(packageSizeBuffer, 0, packageSizeBuffer.Length);
                            int totalReceived = 0;
                            while (totalReceived != requestSize)
                            {
                                byte[] package = new byte[bufferSize];

                                int received = client.Receive(package, SocketFlags.None);
                                _logger.Info($"Received {received} bytes");

                                ms.Write(package, 0, received);
                                totalReceived += received;
                                _logger.Info($"Total received bytes: {(float)totalReceived * 100 / requestSize}%");
                            }
                            r = ms.ToArray();
                        }

                        timer.Stop();
                        _logger.Info($"Finished receiving package in {timer.ElapsedMilliseconds}ms");

                        _logger.Info("Processing request");
                        _processor.ProcessRequest(client, r, Storage);
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
                            if (client != null && client.Connected)
                            {
                                client.Shutdown(SocketShutdown.Both);
                            }
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
            Storage.Clear();
        }
    }
}
