using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kronos.Client.Transfer;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;

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

        private readonly ServerProvider _serverProvider;
        private readonly ConcurrentPool<Connection> _connectionPool = new ConcurrentPool<Connection>();

        public KronosClient(KronosConfig config)
        {
            _serverProvider = new ServerProvider(config.ClusterConfig);
        }

        public async Task InsertAsync(string key, byte[] package, DateTime expiryDate)
        {
            Trace.WriteLine("New insert request");
            InsertRequest request = new InsertRequest(key, package, expiryDate);

            ServerConfig server = GetServerInternal(key);

            bool response = await _connectionPool.UseAsync(con =>
            {
                return _insertProcessor.ExecuteAsync(request, con, server);
            });

            Trace.WriteLine($"InsertRequest status: {response}");
        }

        public async Task<byte[]> GetAsync(string key)
        {
            Trace.WriteLine("New get request");
            GetRequest request = new GetRequest(key);

            ServerConfig server = GetServerInternal(key);

            byte[] valueFromCache = await _connectionPool.UseAsync(con =>
            {
                return _getProcessor.ExecuteAsync(request, con, server);
            });

            byte[] notFoundBytes = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            if (valueFromCache != null && valueFromCache.SequenceEqual(notFoundBytes))
                return null;

            return valueFromCache;
        }

        public async Task DeleteAsync(string key)
        {
            Trace.WriteLine("New delete request");
            DeleteRequest request = new DeleteRequest(key);

            ServerConfig server = GetServerInternal(key);

            bool status = await _connectionPool.UseAsync(con =>
            {
                return _deleteProcessor.ExecuteAsync(request, con, server);
            });
            
            Trace.WriteLine($"InsertRequest status: {status}");
        }

        public async Task<int> CountAsync()
        {
            Trace.WriteLine("New count request");

            var request = new CountRequest();
            ServerConfig[] servers = GetServersInternal();

            int[] results = await _connectionPool.UseAsync(servers.Length, con =>
            {
                return _countProcessor.ExecuteAsync(request, con, servers);
            });

            return results.Sum();
        }

        public async Task<bool> ContainsAsync(string key)
        {
            Trace.WriteLine("New contains request");

            var request = new ContainsRequest(key);

            ServerConfig server = GetServerInternal(key);

            bool contains = await _connectionPool.UseAsync(con =>
            {
                return _containsProcessor.ExecuteAsync(request, con, server);
            });
            
            return contains;
        }

        public async Task ClearAsync()
        {
            Trace.WriteLine("New clear request");

            var request = new ClearRequest();
            ServerConfig[] servers = GetServersInternal();

            await _connectionPool.UseAsync(servers.Length, con =>
            {
                return _clearProcessor.ExecuteAsync(request, con, servers);
            });
        }

        private ServerConfig GetServerInternal(string key)
        {
            return _serverProvider.SelectServer(key.GetHashCode());
        }

        private ServerConfig[] GetServersInternal()
        {
            return _serverProvider.Servers.ToArray();
        }
    }
}

