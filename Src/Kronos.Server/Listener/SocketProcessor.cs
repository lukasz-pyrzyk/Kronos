using System;
using System.IO;
using System.Linq;
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
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Task<MessageArgs> ProcessSocketConnectionAsync(ISocket client)
        {
            _logger.Debug("Accepting new request");
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
            byte[] lengthBuffer = new byte[sizeof(int)];
            ReceiveBytes(socket, lengthBuffer);
            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] typeBuffer = new byte[sizeof(RequestType)];
            ReceiveBytes(socket, typeBuffer);

            byte[] data = new byte[dataLength - typeBuffer.Length];
            ReceiveBytes(socket, data);

            RequestType requestType = SerializationUtils.Deserialize<RequestType>(typeBuffer);

            return new ReceivedMessage { Type = requestType, Data = data };
        }

        private void ReceiveBytes(ISocket socket, byte[] buffer)
        {
            if (buffer.Length <= socket.BufferSize)
            {
                socket.Receive(buffer);
                return;
            }

            int position = 0;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                while (position != buffer.Length)
                {
                    byte[] buf = new byte[socket.BufferSize];
                    int received = socket.Receive(buf);
                    ms.Write(buf, 0, received);
                    position += received;
                }
            }
        }

        struct ReceivedMessage
        {
            public RequestType Type { get; set; }
            public byte[] Data { get; set; }
        }
    }
}
