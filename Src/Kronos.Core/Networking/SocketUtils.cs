using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Networking
{
    public static class SocketUtils
    {
        private const int BufferSize = 8 * 1024;

        public static void SendAll(Socket socket, byte[] data)
        {
            SendAll(socket, data, data.Length);
        }

        public static void SendAll(Socket socket, byte[] data, int count)
        {
            int position = 0;
            while (position != count)
            {
                int size = Math.Min(count - position, BufferSize);
                int sent = socket.Send(data, position, size, SocketFlags.None);
                position += sent;

                Debug.Assert(position <= data.Length);
            }
        }

        //public static async Task SendAllAsync(Socket socket, byte[] data, int count)
        //{
        //    int position = 0;
        //    while (position != count)
        //    {
        //        int size = Math.Min(count - position, BufferSize);
        //        int sent = await socket.SendAsync(new ArraySegment<byte>(data, position, size), SocketFlags.None).ConfigureAwait(false);
        //        position += sent;

        //        Debug.Assert(position <= data.Length);
        //    }
        //}

        public static Task SendAllAsync(Socket socket, byte[] data, int count)
        {
            return Task.Run(() => SendAll(socket, data));
        }

        public static void ReceiveAll(Socket socket, byte[] data, int count)
        {
            int position = 0;
            while (position != count)
            {
                int size = Math.Min(count - position, BufferSize);
                int received = socket.Receive(data, position, size, SocketFlags.None);
                position += received;

                Debug.Assert(position <= count);
            }
        }

        public static Task ReceiveAllAsync(Socket socket, byte[] data, int count)
        {
            return Task.Run(() => ReceiveAll(socket, data, count));
        }

        //public static async Task ReceiveAllAsync(Socket socket, byte[] data, int count)
        //{
        //    int position = 0;
        //    while (position != count)
        //    {
        //        int size = Math.Min(count - position, BufferSize);
        //        var segment = new ArraySegment<byte>(data, position, size);
        //        int received = await socket.ReceiveAsync(segment, SocketFlags.None).ConfigureAwait(false);
        //        position += received;

        //        Debug.Assert(position <= count);
        //    }
        //}
    }
}
