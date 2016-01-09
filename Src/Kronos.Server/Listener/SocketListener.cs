using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

                while (true)
                {
                    Socket handler = server.Accept();

                    byte[] packageSizeBuffer = new byte[sizeof(int)];
                    handler.Receive(packageSizeBuffer, SocketFlags.None);
                    int packageSize = BitConverter.ToInt32(packageSizeBuffer, 0);

                    byte[] data = new byte[packageSize];
                    int readedBytes = sizeof(int);
                    while (readedBytes != packageSize)
                    {
                        byte[] package = new byte[1024];
                        int received = handler.Receive(package, SocketFlags.None);
                        // TODO join all packages
                        readedBytes += received;
                    }
                    handler.Send(BitConverter.GetBytes(readedBytes));
                    handler.Dispose();
                }
            }
            catch (SocketException)
            {
                // TODO
            }
            finally
            {
                server?.Dispose();
            }
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
