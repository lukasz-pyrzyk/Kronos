using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading.Tasks;
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
            ReceiveUntilFullBuffer(socket, lengthBuffer, IntSize);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);
            _bytesPool.Return(lengthBuffer);

            byte[] typeBuffer = _bytesPool.Rent(RequestTypeSize);
            ReceiveUntilFullBuffer(socket, typeBuffer, RequestTypeSize);
            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer, RequestTypeSize);
            _bytesPool.Return(typeBuffer);

            byte[] data = new byte[dataLength - RequestTypeSize];
            ReceiveUntilFullBuffer(socket, data, data.Length);

            return new ReceivedMessage(requestType, data);
        }

        private void ReceiveUntilFullBuffer(ISocket socket, byte[] buffer, int count)
        {
            int position = 0;
            while (position != count)
            {
                int expectedSize = Math.Min(count - position, socket.BufferSize);
                int received = socket.Receive(buffer, position, expectedSize, SocketFlags.None);
                position += received;
            }
        }
    }
}
