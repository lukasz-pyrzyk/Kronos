using System;
using System.Buffers;
using System.Net.Sockets;
using Google.Protobuf;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;

namespace Kronos.Server.Listening
{
    public class SocketProcessor : IProcessor
    {
        private const int IntSize = sizeof(int);
        private readonly byte[] sizeBuffer = new byte[sizeof(int)];

        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create();
        private readonly BufferedStream _stream = new BufferedStream();

        public RequestArg ReceiveRequest(Socket client)
        {
            int packageSize;
            SocketUtils.ReceiveAll(client, sizeBuffer, IntSize);
            packageSize = BitConverter.ToInt32(sizeBuffer, 0);
            Array.Clear(sizeBuffer, 0, 0);

            byte[] data = _pool.Rent(packageSize);
            Request response;
            try
            {
                SocketUtils.ReceiveAll(client, data, packageSize);
                response = Request.Parser.ParseFrom(new CodedInputStream(data, 0, packageSize));
            }
            finally
            {
                _pool.Return(data);
            }

            return new RequestArg(response, client);
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
