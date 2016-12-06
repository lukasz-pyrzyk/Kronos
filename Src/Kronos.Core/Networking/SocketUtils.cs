using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Networking
{
    public static class SocketUtils
    {
        private const int BufferSize = 8*1024;

        public static void SendAll(Socket socket, byte[] data)
        {
            int position = 0;
            while (position != data.Length)
            {
                int size = CalculateBufferSize(data, position);
                int sent = socket.Send(data, position, size, SocketFlags.None);
                position += sent;

                Debug.Assert(position <= data.Length);
            }
        }

        public static async Task ReceiveAllAsync(Socket socket, byte[] data, int count)
        {
            int position = 0;
            while (position != count)
            {
                int size =  CalculateBufferSize(data, position);
                var segment = new ArraySegment<byte>(data, position, size);
                int received = await socket.ReceiveAsync(segment, SocketFlags.None);
                position += received;

                Debug.Assert(position <= count);
            }
        }

        private static int CalculateBufferSize(byte[] data, int position)
        {
            return Math.Min(data.Length - position, BufferSize);
        }
    }
}
