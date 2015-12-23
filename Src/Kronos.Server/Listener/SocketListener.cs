using System;
using System.Net;
using System.Net.Sockets;

namespace Kronos.Server.Listener
{
    public class SocketListener : ICommunicationListener
    {
        public const int QueueSize = 5;
        public const int Port = 7;
        public const int BufferSize = 5555;

        public void StartListening()
        {
            Socket server = null;
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, Port));
                server.Listen(QueueSize);
            }
            catch (SocketException ex)
            {
                Environment.Exit(ex.ErrorCode);
            }

            byte[] rcvBuffer = new byte[BufferSize];
            int rcvbytes = 0;
            long totalBytes = 0;

            bool workingServer = true;
            while (workingServer)
            {
                Socket client = null;
                try
                {
                    client = server.Accept();
                    client.ReceiveTimeout = 40;
                    while (client.Poll(40, SelectMode.SelectRead))
                    {
                        rcvbytes = client.Receive(rcvBuffer, 0, BufferSize, SocketFlags.None);
                        totalBytes += rcvbytes;
                        Console.WriteLine("Received {0} bytes", rcvbytes);
                    }
                    byte[] packageToResentToTheClient = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(totalBytes));
                    client.Send(packageToResentToTheClient, 0, packageToResentToTheClient.Length, SocketFlags.None);
                }
                catch (Exception)
                {
                    workingServer = false;
                }
                finally
                {
                    client?.Dispose();
                    server?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
