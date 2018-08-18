using System;
using System.Buffers;
using System.Net.Sockets;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Serialization;

namespace Kronos.Server
{
    public class SocketProcessor : ISocketProcessor
    {
        private readonly byte[] _sizeBuffer = new byte[sizeof(int)];

        public Request ReceiveRequest(Socket client)
        {
            SocketUtils.ReceiveAll(client, _sizeBuffer, _sizeBuffer.Length);
            int packageSize = BitConverter.ToInt32(_sizeBuffer, 0);
            Array.Clear(_sizeBuffer, 0, _sizeBuffer.Length);

            byte[] data = ArrayPool<byte>.Shared.Rent(packageSize);
            Request request = new Request();
            try
            {
                SocketUtils.ReceiveAll(client, data, packageSize);
                request.Read(new DeserializationStream(data));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }

            return request;
        }

        public void SendResponse(Socket client, Response response)
        {
            int neededSize = response.CalculateSize();
            using (var stream = new SerializationStream(neededSize))
            {
                response.Write(stream);
                stream.Flush();

                SocketUtils.SendAll(client, stream.MemoryWithLengthPrefix.Span);
            }
        }
    }
}
