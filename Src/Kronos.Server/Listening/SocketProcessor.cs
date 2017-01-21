using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
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

        public async Task<RequestArgs> ReceiveRequestAsync(NetworkStream stream)
        {
            byte[] lengthBuffer = ArrayPool<byte>.Shared.Rent(IntSize); // TODO stackalloc
            await SocketUtils.ReceiveAllAsync(stream, lengthBuffer, IntSize).ConfigureAwait(false);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
            ArrayPool<byte>.Shared.Return(lengthBuffer);
            Debug.Assert(dataLength != 0);

            byte[] typeBuffer = ArrayPool<byte>.Shared.Rent(RequestTypeSize); // todo stackalloc;
            await SocketUtils.ReceiveAllAsync(stream, typeBuffer, RequestTypeSize).ConfigureAwait(false);
            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer, RequestTypeSize);
            ArrayPool<byte>.Shared.Return(typeBuffer);
            Debug.Assert(requestType != RequestType.Unknown);

            int packageSize = dataLength - RequestTypeSize;
            byte[] data = ArrayPool<byte>.Shared.Rent(packageSize);
            await SocketUtils.ReceiveAllAsync(stream, data, packageSize).ConfigureAwait(false);
            return new RequestArgs(requestType, data, packageSize);
        }
    }
}
