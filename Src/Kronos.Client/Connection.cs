using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Client.Configuration;
using Kronos.Core;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;

namespace Kronos.Client
{
    public class Connection
    {
        private readonly SocketConnection _connection = new SocketConnection();

        public async Task<Response> SendAsync(Request request, ServerConfig server)
        {
            TcpClient client = null;
            Response response;
            try
            {
                Trace.WriteLine("Connecting to the server socket");
                client = new TcpClient();
                await client.ConnectAsync(server.EndPoint.Address, server.Port).ConfigureAwait(false);
                var stream = client.GetStream();

                Trace.WriteLine("Sending request");
                await _connection.Send(request, stream).ConfigureAwait(false);

                Trace.WriteLine("Waiting for response");
                response = _connection.ReceiveResponse(stream);
            }
            catch (Exception ex)
            {
                throw new KronosCommunicationException($"Connection to the {server.EndPoint} has been refused", ex);
            }
            finally
            {
                client?.Dispose();
            }

            return response;
        }
    }
}