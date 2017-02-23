using System;
using System.Buffers;
using System.Net.Sockets;
using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;

namespace Kronos.Server.Listening
{
    public class SocketProcessor : IProcessor
    {
        private const int IntSize = sizeof(int);
        private readonly byte[] _sizeBuffer = new byte[sizeof(int)];

        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create();
        private readonly BufferedStream _stream = new BufferedStream();

        public Request ReceiveRequest(Socket client)
        {
            int packageSize;
            SocketUtils.ReceiveAll(client, _sizeBuffer, IntSize);
            packageSize = BitConverter.ToInt32(_sizeBuffer, 0);
            Array.Clear(_sizeBuffer, 0, _sizeBuffer.Length);

            byte[] data = _pool.Rent(packageSize);
            Request request;
            try
            {
                SocketUtils.ReceiveAll(client, data, packageSize);
                request = Request.Parser.ParseFrom(new CodedInputStream(data, 0, packageSize));
            }
            finally
            {
                _pool.Return(data);
            }

            return request;
        }

        public void SendResponse(Socket client, Response response)
        {
            response.WriteTo(_stream);

            try
            {
                SocketUtils.SendAll(client, _stream.RawBytes, (int)_stream.Length);
            }
            finally
            {
                _stream.Clean();
            }
        }
    }
}
