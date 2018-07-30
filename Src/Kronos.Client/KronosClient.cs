using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
using Kronos.Core.Pooling;
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

        private readonly ServerProvider _serverProvider;
        private readonly ConcurrentPool<Connection> _connectionPool = new ConcurrentPool<Connection>();

        public KronosClient(KronosConfig config)
        {
            _serverProvider = new ServerProvider(config.ClusterConfig);
        }

        public async Task<bool> InsertAsync(string key, byte[] package, DateTime? expiryDate)
        {
            Trace.WriteLine("New insert request");

            ServerConfig server = GetServerInternal(key);
            Request request = InsertRequest.New(key, package, expiryDate, server.Auth);

            var response = await _connectionPool.UseAsync(con => _insertProcessor.ExecuteAsync(request, con, server));

            Trace.WriteLine($"InsertRequest status: {response.Added}");

            return response.Added;
        }

        public async Task<byte[]> GetAsync(string key)
        {
            Trace.WriteLine("New get request");

            ServerConfig server = GetServerInternal(key);
            Request request = GetRequest.New(key, server.Auth);

            var response = await _connectionPool.UseAsync(con => _getProcessor.ExecuteAsync(request, con, server));

            if (response.Exists)
                return response.Data.ToByteArray();

            return null;
        }

        public async Task DeleteAsync(string key)
        {
            Trace.WriteLine("New delete request");

            ServerConfig server = GetServerInternal(key);
            Request request = DeleteRequest.New(key, server.Auth);

            var response = await _connectionPool.UseAsync(con => _deleteProcessor.ExecuteAsync(request, con, server));

            Debug.WriteLine($"InsertRequest status: {response.Deleted}");
        }

        public async Task<int> CountAsync()
        {
            Trace.WriteLine("New count request");

            ServerConfig[] servers = GetServersInternal();
            Request[] requests = servers.Select(x => CountRequest.New(x.Auth)).ToArray();

            var responses = await _connectionPool.UseAsync(servers.Length, con => _countProcessor.ExecuteAsync(requests, con, servers));

            return responses.Sum(x => x.Count);
        }

        public async Task<bool> ContainsAsync(string key)
        {
            Trace.WriteLine("New contains request");

            ServerConfig server = GetServerInternal(key);
            Request request = ContainsRequest.New(key, server.Auth);

            var response = await _connectionPool.UseAsync(con => _containsProcessor.ExecuteAsync(request, con, server));

            return response.Contains;
        }


        public async Task<StatsResponse[]> StatsAsync()
        {
            Trace.WriteLine("New stats request");

            var servers = GetServersInternal();
            var requests = servers.Select(x => StatsRequest.New(x.Auth)).ToArray();

            var responses = await _connectionPool.UseAsync(servers.Length, con => _statsProcessor.ExecuteAsync(requests, con, servers));
            return responses;
        }

        public async Task ClearAsync()
        {
            Trace.WriteLine("New clear request");

            ServerConfig[] servers = GetServersInternal();
            Request[] requests = servers.Select(x => ClearRequest.New(x.Auth)).ToArray();

            await _connectionPool.UseAsync(servers.Length, con => _clearProcessor.ExecuteAsync(requests, con, servers));
        }

        private ServerConfig GetServerInternal(string key)
        {
            return _serverProvider.SelectServer(key.GetHashCode());
        }

        private ServerConfig[] GetServersInternal()
        {
            return _serverProvider.Servers;
        }
    }
}

