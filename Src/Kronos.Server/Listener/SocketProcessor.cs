using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using NLog;
using XGain;
using XGain.Processing;
using XGain.Sockets;

namespace Kronos.Server.Listener
{
    public class SocketProcessor : IProcessor<Message>
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public async void ProcessSocketConnection(ISocket client, Message args)
        {
            try
            {
                _logger.Info("Accepting new request");

                byte[] typeBuffer = await ReceiveAndSendConfirmation(client);
                byte[] requestBuffer = await ReceiveAndSendConfirmation(client);

                RequestType type = SerializationUtils.Deserialize<RequestType>(typeBuffer);

                args.RequestBytes = requestBuffer;
                args.UserToken = type;
                args.Client = client;
            }
            catch (SocketException ex)
            {
                _logger.Error(
                    $"Exception during receiving request from client {client?.RemoteEndPoint}");
                _logger.Fatal(ex);
            }
        }

        private async Task<byte[]> ReceiveAndSendConfirmation(ISocket socket)
        {
            byte[] packageSizeBuffer = new byte[sizeof(int)];
            _logger.Info("Receiving information about request size");
            int position = socket.Receive(packageSizeBuffer);

            int requestSize = SerializationUtils.GetLengthOfPackage(packageSizeBuffer);
            _logger.Info($"Request contains {requestSize} bytes");

            using (MemoryStream ms = new MemoryStream())
            {
                await ms.WriteAsync(packageSizeBuffer, 0, position);
                position = 0;
                while (position != requestSize)
                {
                    byte[] package = new byte[socket.BufferSize];

                    int received = socket.Receive(package);
                    _logger.Info($"Received {received} bytes");

                    await ms.WriteAsync(package, 0, received);
                    position += received;
                    _logger.Info($"Total received bytes: {(float)position * 100 / requestSize}%");
                }

                // send confirmation
                byte[] statusBuffer = SerializationUtils.Serialize(RequestStatusCode.Ok);
                socket.Send(statusBuffer);

                return ms.ToArray();
            }
        }
    }
}
