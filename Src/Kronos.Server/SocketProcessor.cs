using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Google.Protobuf;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;

namespace Kronos.Server
{
    public class SocketProcessor : ISocketProcessor
    {
        private readonly byte[] _sizeBuffer = new byte[sizeof(int)];

        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create();
        private readonly BufferedStream _stream = new BufferedStream(Settings.MaxRequestSize);

        public Request ReceiveRequest(Socket client)
        {
            SocketUtils.ReceiveAll(client, _sizeBuffer, _sizeBuffer.Length);
            int packageSize = BitConverter.ToInt32(_sizeBuffer, 0);
            Array.Clear(_sizeBuffer, 0, _sizeBuffer.Length);

            byte[] data = _pool.Rent(packageSize);
            Request request;
            try
            {
                SocketUtils.ReceiveAll(client, data, packageSize);
                request = Request.Parser.ParseFrom(data, 0, packageSize);
                Debug.Assert(request.Type != RequestType.Unknown, "Received invalid request");
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
            _stream.Flush();

            try
            {
                SocketUtils.SendAll(client, _stream.RawBytes, (int)_stream.Position);
            }
            finally
            {
                _stream.Clean();
            }
        }
    }
}
