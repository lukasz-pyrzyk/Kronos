using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Kronos.Core.Requests;
using NLog;

namespace Kronos.Server.Listener
{
    public class SocketListener : ICommunicationListener
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public const int QueueSize = 5;
        public const int Port = 7;
        public const int BufferSize = 5555;
        private const int packageSize = 1024 * 1024;

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
                    Socket connectionRequest = null;
                    try
                    {
                        connectionRequest = server.Accept();
                        var timer = Stopwatch.StartNew();
                        _logger.Info("Accepting new request");

                        byte[] packageSizeBuffer = new byte[sizeof (int)];
                        _logger.Info("Receiving information about request size");
                        connectionRequest.Receive(packageSizeBuffer, SocketFlags.None);

                        int requestSize = BitConverter.ToInt32(packageSizeBuffer, 0) + sizeof (int);
                        _logger.Info($"Request contains {requestSize} bytes");

                        byte[] requestPackage = new byte[requestSize];
                        Buffer.BlockCopy(packageSizeBuffer, 0, requestPackage, 0, packageSizeBuffer.Length);
                        int offset = packageSizeBuffer.Length;
                        while (offset != requestSize)
                        {
                            byte[] package = new byte[packageSize];

                            _logger.Info($"Receiving {packageSize} bytes");
                            int received = connectionRequest.Receive(package, SocketFlags.None);
                            if (package.Length != received)
                            {
                                package = package.Take(received).ToArray();
                            }
                            Buffer.BlockCopy(package, 0, requestPackage, offset, package.Length);
                            offset += received;
                            _logger.Info($"Total received bytes: {(float) offset*100/requestSize}%");
                        }

                        timer.Stop();
                        _logger.Info($"Finished receiving package in {timer.ElapsedMilliseconds}ms");

                        _logger.Info("Sending response to the client");
                        
                        connectionRequest.Shutdown(SocketShutdown.Send);
                        connectionRequest.Send(BitConverter.GetBytes(offset));

                        InsertRequest request = InsertRequest.Deserialize(requestPackage);
                    }
                    catch (SocketException ex)
                    {
                        _logger.Error(
                            $"Exception during receiving request from client {connectionRequest?.RemoteEndPoint}");
                        _logger.Fatal(ex);
                    }
                    finally
                    {
                        connectionRequest?.Shutdown(SocketShutdown.Both);
                        connectionRequest?.Dispose();
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
            // TODO
        }
    }
}
