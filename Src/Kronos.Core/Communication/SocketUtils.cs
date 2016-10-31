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

                int sent = socket.Send(data, sentbytes, sizeToSend, SocketFlags.None);
                sentbytes += sent;
            }
        }
    }
}
