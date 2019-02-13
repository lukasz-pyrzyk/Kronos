using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
using Kronos.Core.Processing;

namespace Kronos.Client
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly InsertProcessor _insertProcessor = new InsertProcessor();
        private readonly GetProcessor _getProcessor = new GetProcessor();
        private readonly DeleteProcessor _deleteProcessor = new DeleteProcessor();
        private readonly CountProcessor _countProcessor = new CountProcessor();
        private readonly ContainsProcessor _containsProcessor = new ContainsProcessor();
        private readonly ClearProcessor _clearProcessor = new ClearProcessor();
        private readonly StatsProcessor _statsProcessor = new StatsProcessor();

        private readonly Connection _connection = new Connection();

        private readonly ServerConfig _server;

        public KronosClient(KronosConfig config)
        {
            _server = config.Server;
        }

        public async Task<bool> InsertAsync(string key, byte[] package, DateTime? expiryDate)
        {
            Trace.WriteLine("New insert request");

            Request request = InsertRequest.New(key, package, expiryDate, _server.Auth);

            var response = await _insertProcessor.ExecuteAsync(request, _connection, _server);

            Trace.WriteLine($"InsertRequest status: {response.Added}");

            return response.Added;
        }

        public async Task<byte[]> GetAsync(string key)
        {
            Trace.WriteLine("New get request");

            Request request = GetRequest.New(key, _server.Auth);

            var response = await _getProcessor.ExecuteAsync(request, _connection, _server);

            if (response.Exists)
                return response.Data.ToByteArray();

            return null;
        }

        public async Task DeleteAsync(string key)
        {
            Trace.WriteLine("New delete request");

            Request request = DeleteRequest.New(key, _server.Auth);

            var response = await _deleteProcessor.ExecuteAsync(request, _connection, _server);

            Debug.WriteLine($"InsertRequest status: {response.Deleted}");
        }

        public async Task<int> CountAsync()
        {
            Trace.WriteLine("New count request");

            var request = CountRequest.New(_server.Auth);

            var response = await _countProcessor.ExecuteAsync(request, _connection, _server);

            return response.Count;
        }

        public async Task<bool> ContainsAsync(string key)
        {
            Trace.WriteLine("New contains request");

            Request request = ContainsRequest.New(key, _server.Auth);

            var response = await _containsProcessor.ExecuteAsync(request, _connection, _server);

            return response.Contains;
        }


        public async Task<StatsResponse> StatsAsync()
        {
            Trace.WriteLine("New stats request");

            var requests = StatsRequest.New(_server.Auth);

            var response = await _statsProcessor.ExecuteAsync(requests, _connection, _server);
            return response;
        }

        public async Task ClearAsync()
        {
            Trace.WriteLine("New clear request");

            var request = ClearRequest.New(_server.Auth);

            await _clearProcessor.ExecuteAsync(request, _connection, _server);
        }
    }
}

