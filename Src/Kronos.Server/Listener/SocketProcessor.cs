using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using NLog;
using XGain;
using XGain.Processing;
using XGain.Sockets;

namespace Kronos.Server.Listener
{
    public class SocketProcessor : IProcessor<MessageArgs>
    {
        private const int IntSize = sizeof(int);
        private const int RequestTypeSize = sizeof(ushort);

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ArrayPool<byte> _bytesPool = ArrayPool<byte>.Create();

        public Task<MessageArgs> ProcessSocketConnectionAsync(ISocket client)
        {
            MessageArgs args = null;
            try
            {
                ReceivedMessage msg = ReceiveMessageAsync(client);
                args = new MessageArgs(client, msg.Data, msg.Type);
            }
            catch (SocketException ex)
            {
                _logger.Error(
                    $"Exception during receiving request from client {client?.RemoteEndPoint} + {ex}");
            }

            return Task.FromResult(args);
        }

        private ReceivedMessage ReceiveMessageAsync(ISocket socket)
        {
            byte[] lengthBuffer = _bytesPool.Rent(IntSize);
            SocketUtils.ReceiveAll(socket, lengthBuffer, IntSize);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
            _bytesPool.Return(lengthBuffer);

            byte[] typeBuffer = _bytesPool.Rent(RequestTypeSize);
            SocketUtils.ReceiveAll(socket, typeBuffer, RequestTypeSize);
            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer, RequestTypeSize);
            _bytesPool.Return(typeBuffer);

            byte[] data = new byte[dataLength - RequestTypeSize];
            SocketUtils.ReceiveAll(socket, data, data.Length);

            return new ReceivedMessage(requestType, data);
        }
    }
}
