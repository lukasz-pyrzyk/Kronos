using System;
using Kronos.Client.Core.Server;
using Kronos.Shared.Configuration;
using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Model;
using Kronos.Shared.Network.Requests;

namespace Kronos.Client.Core
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly ICommunicationService _service;
        private readonly IServerConfiguration _configuration;

        public KronosClient(ICommunicationService service, IServerConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        public RequestStatusCode InsertToServer(string key, byte[] package, DateTime expiryDate)
        {
            CachedObject objectToCache = new CachedObject(key, package, expiryDate);

            return InsertToKronosNode(objectToCache);
        }

        public RequestStatusCode InsertToServer(CachedObject objectToCache)
        {
            return InsertToKronosNode(objectToCache);
        }

        public void Dispose()
        {
            // TODO
        }

        private RequestStatusCode InsertToKronosNode(CachedObject objectToCache)
        {
            NodeConfiguration nodeConfiguration = _configuration.GetNodeForStream(objectToCache);
            InsertRequest request = new InsertRequest(objectToCache);
            RequestStatusCode result = _service.SendToNode(request, nodeConfiguration.Endpoint);
            return result;
        }
    }
}
