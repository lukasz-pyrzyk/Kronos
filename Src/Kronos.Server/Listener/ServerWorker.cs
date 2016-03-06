using System;
using System.IO;
using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.RequestProcessing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using NLog;

namespace Kronos.Server.Listener
{
    internal class SocketServerWorker : IServerWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestProcessor _processor;
        public IStorage Storage { get; }

        internal SocketServerWorker(IRequestProcessor processor, IStorage storage)
        {
            _processor = processor;
            Storage = storage;
        }

        public SocketServerWorker() : this(new RequestProcessor(), new InMemoryStorage())
        {
        }

        public void StartListening(ISocket server)
        {
            try
            {
                while (true)
                {
                    ISocket client = null;
                    try
                    {
                        client = server.Accept();
                        _logger.Info("Accepting new request");

                        byte[] typeBuffer = ReceiveAndSendConfirmation(client);
                        byte[] requestBuffer = ReceiveAndSendConfirmation(client);

                        RequestType type = SerializationUtils.Deserialize<RequestType>(typeBuffer);

                        _logger.Info($"Processing {type} request");
                        _processor.ProcessRequest(client, requestBuffer, type, Storage);
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

        private byte[] ReceiveAndSendConfirmation(ISocket socket)
        {
            byte[] packageSizeBuffer = new byte[sizeof(int)];
            _logger.Info("Receiving information about request size");
            socket.Receive(packageSizeBuffer);

            int requestSize = SerializationUtils.GetLengthOfPackage(packageSizeBuffer);
            _logger.Info($"Request contains {requestSize} bytes");

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(packageSizeBuffer, 0, packageSizeBuffer.Length);
                int totalReceived = 0;
                while (totalReceived != requestSize)
                {
                    byte[] package = new byte[socket.BufferSize];

                    int received = socket.Receive(package);
                    _logger.Info($"Received {received} bytes");

                    ms.Write(package, 0, received);
                    totalReceived += received;
                    _logger.Info($"Total received bytes: {(float)totalReceived * 100 / requestSize}%");
                }

                // send confirmation
                byte[] statusBuffer = SerializationUtils.Serialize(RequestStatusCode.Ok);
                socket.Send(statusBuffer);

                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            Storage.Clear();
        }
    }
}
