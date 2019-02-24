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
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly SocketConnection _connection = new SocketConnection();

        private readonly ServerConfig _server;

        public KronosClient(KronosConfig config)
        {
            _server = config.Server;
        }

        public async Task<InsertResponse> InsertAsync(string key, byte[] package, DateTimeOffset? expiryDate)
        {
            Trace.WriteLine("New insert request");

            var request = InsertRequest.New(key, package, expiryDate, _server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.InsertResponse != null)
            {
                Trace.WriteLine($"InsertRequest finished successfully. Element added: {response.InsertResponse.Added}");
                return response.InsertResponse;
            }

            Trace.TraceError($"InsertRequest failed. Exception: {response.Exception}");
            return null;
        }

        public async Task<GetResponse> GetAsync(string key)
        {
            Trace.WriteLine("New get request");

            var request = GetRequest.New(key, _server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.GetRespone != null)
            {
                Trace.WriteLine("GetRRequest finished successfully");
                return response.GetRespone;
            }

            Trace.TraceError($"GetRequest failed. Exception: {response.Exception}");
            return null;
        }

        public async Task<DeleteResponse> DeleteAsync(string key)
        {
            Trace.WriteLine("New delete request");

            var request = DeleteRequest.New(key, _server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.DeleteResponse != null)
            {
                Trace.WriteLine($"DeleteRequest finished successfully. Element deleted: {response.DeleteResponse.Deleted}");
                return response.DeleteResponse;
            }

            Trace.TraceError($"DeleteRequest failed. Exception: {response.Exception}");
            return null;
        }

        public async Task<CountResponse> CountAsync()
        {
            Trace.WriteLine("New count request");

            var request = CountRequest.New(_server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.CountResponse != null)
            {
                Trace.WriteLine($"CountRequest finished successfully. Elements: {response.CountResponse.Count}");
                return response.CountResponse;
            }

            Trace.TraceError($"CountRequest failed. Exception: {response.Exception}");
            return null;
        }

        public async Task<ContainsResponse> ContainsAsync(string key)
        {
            Trace.WriteLine("New contains request");

            var request = ContainsRequest.New(key, _server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.ContainsResponse != null)
            {
                Trace.WriteLine($"ContainsRequest finished successfully. Element {key} contains: {response.ContainsResponse.Contains}");
                return response.ContainsResponse;
            }

            Trace.TraceError($"ContainsRequest failed. Exception: {response.Exception}");
            return null;
        }

        public async Task<StatsResponse> StatsAsync()
        {
            Trace.WriteLine("New stats request");

            var request = StatsRequest.New(_server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.StatsResponse != null)
            {
                Trace.WriteLine($"StatsRequest finished successfully with status {response.StatsResponse.Elements} elements with total {response.StatsResponse.MemoryUsed} memory used");
                return response.StatsResponse;
            }

            Trace.TraceError($"StatsRequest failed. Exception: {response.Exception}");
            return null;
        }

        public async Task<ClearResponse> ClearAsync()
        {
            Trace.WriteLine("New clear request");

            var request = ClearRequest.New(_server.Auth);
            var response = await SendAsync(request, _server);
            if (response.Success && response.ClearResponse != null)
            {
                Trace.WriteLine($"ClearRequest finished successfully, deleted {response.ClearResponse.Deleted} items");
                return response.ClearResponse;
            }

            Trace.TraceError($"ClearRequest failed. Exception: {response.Exception}");
            return null;
        }

        private async Task<Response> SendAsync(Request request, ServerConfig server)
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

