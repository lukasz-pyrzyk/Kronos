using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kronos.Client.Transfer;
using Kronos.Core.Communication;
using Kronos.Core.Configuration;
using Kronos.Core.Processors;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;

namespace Kronos.Client
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly ServerProvider _serverProvider;
        private readonly Func<IPEndPoint, IClientServerConnection> _connectionResolver;

        private readonly InsertProcessor _insertProcessor = new InsertProcessor();
        private readonly GetProcessor _getProcessor = new GetProcessor();
        private readonly DeleteProcessor _deleteProcessor = new DeleteProcessor();
        private readonly CountProcessor _countProcessor = new CountProcessor();
        private readonly ContainsProcessor _containsProcessor = new ContainsProcessor();

        public KronosClient(KronosConfig config) : this(config, endpoint => new SocketCommunicationService(endpoint))
        {
        }

        internal KronosClient(KronosConfig config, Func<IPEndPoint, IClientServerConnection> connectionResolver)
        {
            _serverProvider = new ServerProvider(config.ClusterConfig);
            _connectionResolver = connectionResolver;
        }

        public async Task InsertAsync(string key, byte[] package, DateTime expiryDate)
        {
            Trace.WriteLine("New insert request");
            InsertRequest request = new InsertRequest(key, package, expiryDate);

            IClientServerConnection connection = SelectServerAndCreateConnection(key);
            bool response = await _insertProcessor.ExecuteAsync(request, connection);

            Trace.WriteLine($"InsertRequest status: {response}");
        }

        public async Task<byte[]> GetAsync(string key)
        {
            Trace.WriteLine("New get request");
            GetRequest request = new GetRequest(key);

            IClientServerConnection connection = SelectServerAndCreateConnection(key);
            byte[] valueFromCache = await _getProcessor.ExecuteAsync(request, connection);

            byte[] notFoundBytes = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            if (valueFromCache != null && valueFromCache.SequenceEqual(notFoundBytes))
                return null;

            return valueFromCache;
        }

        public async Task DeleteAsync(string key)
        {
            Trace.WriteLine("New delete request");
            DeleteRequest request = new DeleteRequest(key);
            IClientServerConnection connection = SelectServerAndCreateConnection(key);
            bool status = await _deleteProcessor.ExecuteAsync(request, connection);

            Trace.WriteLine($"InsertRequest status: {status}");
        }

        public async Task<int> CountAsync()
        {
            Trace.WriteLine("New count request");

            ServerConfig[] servers = _serverProvider.SelectServers();

            Task<int>[] tasks = new Task<int>[servers.Length];
            for (int i = 0; i < servers.Length; i++)
            {
                var server = servers[i];
                var request = new CountRequest();
                IClientServerConnection connection = _connectionResolver(server.EndPoint);
                tasks[i] = _countProcessor.ExecuteAsync(request, connection);
            }

            await Task.WhenAll(tasks);

            return tasks.Sum(x => x.Result);
        }

        public async Task<bool> ContainsAsync(string key)
        {
            ServerConfig[] servers = _serverProvider.SelectServers();

            Task<bool>[] tasks = new Task<bool>[servers.Length];
            for (int i = 0; i < servers.Length; i++)
            {
                var server = servers[i];
                var request = new ContainsRequest(key);
                IClientServerConnection connection = _connectionResolver(server.EndPoint);
                tasks[i] = _containsProcessor.ExecuteAsync(request, connection);
            }

            await Task.WhenAll(tasks);

            return tasks.Any(x => x.Result);
        }

        private IClientServerConnection SelectServerAndCreateConnection(string key)
        {
            ServerConfig server = _serverProvider.SelectServer(key.GetHashCode());
            Trace.WriteLine($"Selected server {server}");
            IClientServerConnection connection = _connectionResolver(server.EndPoint);
            return connection;
        }
    }
}

