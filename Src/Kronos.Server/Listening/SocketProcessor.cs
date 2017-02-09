using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Listening
{
    public class SocketProcessor : IProcessor
    {
        private const int IntSize = sizeof(int);
        private const int RequestTypeSize = sizeof(ushort);

        public RequestArgs ReceiveRequest(Socket client, ArrayPool<byte> pool)
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
            return new RequestArgs(requestType, data, packageSize, client);
        }
    }
}
