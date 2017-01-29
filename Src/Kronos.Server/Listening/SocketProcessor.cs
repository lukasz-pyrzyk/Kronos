﻿using System;
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

        public RequestArgs ReceiveRequest(Socket client)
        {
            byte[] lengthBuffer = ArrayPool<byte>.Shared.Rent(IntSize); // TODO stackalloc
            SocketUtils.ReceiveAll(client, lengthBuffer, IntSize);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
            ArrayPool<byte>.Shared.Return(lengthBuffer);
            Debug.Assert(dataLength != 0);

            byte[] typeBuffer = ArrayPool<byte>.Shared.Rent(RequestTypeSize); // todo stackalloc;
            SocketUtils.ReceiveAll(client, typeBuffer, RequestTypeSize);
            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer, RequestTypeSize);
            ArrayPool<byte>.Shared.Return(typeBuffer);
            Debug.Assert(requestType != RequestType.Unknown);

            int packageSize = dataLength - RequestTypeSize;
            byte[] data = ArrayPool<byte>.Shared.Rent(packageSize);
            SocketUtils.ReceiveAll(client, data, packageSize);
            return new RequestArgs(requestType, data, packageSize, client);
        }
    }
}
