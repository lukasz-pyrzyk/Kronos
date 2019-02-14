﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Client.Configuration;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;

namespace Kronos.Client
{
    public class Connection
    {
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
                await SendAsync(request, stream).ConfigureAwait(false);

                Trace.WriteLine("Waiting for response");
                response = ReceiveAndDeserialize(stream);
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

        private async Task SendAsync(Request request, Stream stream)
        {
            request.WriteDelimitedTo(stream);
            await stream.FlushAsync();
        }

        private Response ReceiveAndDeserialize(Stream stream)
        {
            return Response.Parser.ParseDelimitedFrom(stream);
        }
    }
}