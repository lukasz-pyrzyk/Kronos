using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Networking
{
    public static class SocketUtils
    {
        public const int BufferSize = 64 * 1024;
        public const int Timeout = 10000;

        public static void Prepare(Socket socket)
        {
            socket.ReceiveTimeout = Timeout;
            socket.SendTimeout = Timeout;
        }

        public static void SendAll(Socket socket, ReadOnlySpan<byte> data)
        {
            int position = 0;
            while (position != data.Length)
            {
                int size = Math.Min(data.Length - position, BufferSize);
                int sent = socket.Send(data.Slice(position, size), SocketFlags.None);
                position += sent;

                Debug.Assert(position <= data.Length);
            }
        }

        public static async Task SendAllAsync(Socket socket, ReadOnlyMemory<byte> data)
        {
            int position = 0;
            while (position != data.Length)
            {
                int size = Math.Min(data.Length - position, BufferSize);
                var buffer = data.Slice(position, size);
                int sent = await socket.SendAsync(buffer, SocketFlags.None);
                position += sent;

                Debug.Assert(position <= data.Length);
            }
        }

        public static async Task ReceiveAllAsync(Socket socket, byte[] data, int count)
        {
            int position = 0;
            while (position != count)
            {
                int size = Math.Min(count - position, BufferSize);
                var segment = new ArraySegment<byte>(data, position, size);
                int received = await socket.ReceiveAsync(segment, SocketFlags.None).ConfigureAwait(false);
                position += received;

                Debug.Assert(position <= count);
            }
        }

        public static void ReceiveAll(Socket socket, Memory<byte> buffer)
        {
            int position = 0;
            while (position != buffer.Length)
            {
                int size = Math.Min(buffer.Length - position, BufferSize);
                var span = buffer.Span.Slice(position, size);
                int received = socket.Receive(span, SocketFlags.None);
                position += received;

                Debug.Assert(position <= buffer.Length);
            }
        }
    }
}
