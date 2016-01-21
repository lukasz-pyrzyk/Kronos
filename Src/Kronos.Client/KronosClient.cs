using System;
using Kronos.Client.Transfer;
using Kronos.Core.Configuration;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;
using NLog;

namespace Kronos.Client
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
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
            _logger.Info("New request");

            NodeConfiguration nodeConfiguration = _configuration.GetNodeForStream(objectToCache);
            _logger.Info($"Chosen node: {nodeConfiguration}");

            InsertRequest request = new InsertRequest(objectToCache);

            _logger.Info($"Sending request to communication service");
            RequestStatusCode result = _service.SendToNode(request, nodeConfiguration.Endpoint);
            return result;
        }
    }
}
