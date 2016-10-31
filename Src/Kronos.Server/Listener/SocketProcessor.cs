using System;
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
            byte[] lengthBuffer = new byte[IntSize];
            ReceiveUntilFullBuffer(socket, lengthBuffer);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] typeBuffer = new byte[RequestTypeSize];
            ReceiveUntilFullBuffer(socket, typeBuffer);

            byte[] data = new byte[dataLength - typeBuffer.Length];
            ReceiveUntilFullBuffer(socket, data);

            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer);

            return new ReceivedMessage(requestType, data);
        }

        private void ReceiveUntilFullBuffer(ISocket socket, byte[] buffer)
        {
            int position = 0;
            while (position != buffer.Length)
            {
                int expectedSize = Math.Min(buffer.Length - position, socket.BufferSize);
                int received = socket.Receive(buffer, position, expectedSize, SocketFlags.None);
                position += received;
            }
        }
    }
}
