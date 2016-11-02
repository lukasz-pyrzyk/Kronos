using System;
using System.Net.Sockets;
using XGain.Sockets;

namespace Kronos.Core.Communication
{
    public static class SocketUtils
    {
        public static void SendAll(ISocket socket, byte[] data)
        {
            int sentbytes = 0;
            while (sentbytes != data.Length)
            {
                int sizeToSend = Math.Min(data.Length - sentbytes, socket.BufferSize);

                int sent = socket.Send(data, sentbytes, sizeToSend, SocketFlags.Partial);
                sentbytes += sent;
            }
        }

        public static void ReceiveAll(ISocket socket, byte[] buffer, int count)
        {
            int position = 0;
            while (position != count)
            {
                int expectedSize = Math.Min(count - position, socket.BufferSize);
                int received = socket.Receive(buffer, position, expectedSize, SocketFlags.Partial);
                position += received;
            }
        }
    }
}
