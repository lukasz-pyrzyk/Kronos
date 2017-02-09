using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;

namespace Kronos.Server.Listening
{
    public class SocketProcessor : IProcessor
    {
        private const int IntSize = sizeof(int);
        private const int RequestTypeSize = sizeof(ushort);

        public void ReceiveRequest(Socket client, ref RequestArg args, ArrayPool<byte> pool)
        {
            byte[] lengthBuffer = pool.Rent(IntSize); // TODO stackalloc
            SocketUtils.ReceiveAll(client, lengthBuffer, IntSize);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
            pool.Return(lengthBuffer);
            Debug.Assert(dataLength != 0);

            byte[] typeBuffer = pool.Rent(RequestTypeSize); // todo stackalloc;
            SocketUtils.ReceiveAll(client, typeBuffer, RequestTypeSize);
            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer, RequestTypeSize);
            pool.Return(typeBuffer);
            Debug.Assert(requestType != RequestType.Unknown);

            int packageSize = dataLength - RequestTypeSize;
            byte[] data = pool.Rent(packageSize);
            SocketUtils.ReceiveAll(client, data, packageSize);

            args.Assign(requestType, data, packageSize, client);
        }
    }
}
