using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Google.Protobuf;
using Kronos.Core.Networking;

namespace Kronos.Server.Listening
{
    public class SocketProcessor : IProcessor
    {
        private const int IntSize = sizeof(int);
        private const int RequestTypeSize = sizeof(ushort);

        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create();


        public RequestArg ReceiveRequest(Socket client)
        {
            int dataLength;
            byte[] lengthBuffer = _pool.Rent(IntSize); // TODO stackalloc
            try
            {
                SocketUtils.ReceiveAll(client, lengthBuffer, IntSize);
                dataLength = BitConverter.ToInt32(lengthBuffer, 0);
            }
            finally
            {
                _pool.Return(lengthBuffer);
            }

            int packageSize = dataLength - RequestTypeSize;
            byte[] data = _pool.Rent(packageSize);
            try
            {
                SocketUtils.ReceiveAll(client, data, packageSize);
            }
            finally
            {
                _pool.Return(data);
            }

            Request response = Request.Parser.ParseFrom(ByteString.CopyFrom(data, 0, packageSize));

            return new RequestArg(response, client);
        }
    }
}
