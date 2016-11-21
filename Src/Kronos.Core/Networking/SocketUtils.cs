using System;
using System.Diagnostics;
using System.Net.Sockets;
using XGain.Sockets;

namespace Kronos.Core.Networking
{
    public static class SocketUtils
    {
        public static void SendAll(ISocket socket, byte[] data)
        {
            int position = 0;
            while (position != data.Length)
            {
                int sizeToSend = Math.Min(data.Length - position, socket.BufferSize);

                int sent = socket.Send(data, position, sizeToSend, SocketFlags.None);
                position += sent;

                Debug.Assert(position <= data.Length);
            }
        }

        public static void ReceiveAll(ISocket socket, byte[] buffer, int count)
        {
            int position = 0;
            while (position != count)
            {
                int expectedSize = Math.Min(count - position, socket.BufferSize);
                int received = socket.Receive(buffer, position, expectedSize, SocketFlags.None);
                position += received;

                Debug.Assert(position <= count);
            }
        }
    }
}
