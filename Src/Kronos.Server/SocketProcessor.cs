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

        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create();

        public Request ReceiveRequest(Socket client)
        {
            SocketUtils.ReceiveAll(client, _sizeBuffer, _sizeBuffer.Length);
            int packageSize = BitConverter.ToInt32(_sizeBuffer, 0);
            Array.Clear(_sizeBuffer, 0, _sizeBuffer.Length);

            byte[] data = _pool.Rent(packageSize);
            Request request = new Request();
            try
            {
                SocketUtils.ReceiveAll(client, data, packageSize);
                request.Read(new DeserializationStream(data));
            }
            finally
            {
                _pool.Return(data);
            }

            return request;
        }

        public void SendResponse(Socket client, Response response)
        {
            var stream = new SerializationStream();
            response.Write(stream);
            stream.Flush();

            SocketUtils.SendAll(client, stream.MemoryWithLengthPrefix.Span);
            stream.Dispose();
        }
    }
}
