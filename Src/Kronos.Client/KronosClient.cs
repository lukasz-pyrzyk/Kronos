using System;
using System.Net;
using Kronos.Client.Transfer;
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
        private readonly IPEndPoint _host;

        public KronosClient(ICommunicationService service, IPEndPoint host)
        {
            _service = service;
            _host = host;
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

        public byte[] TryGetValue(string key)
        {
            GetRequest request = new GetRequest(key);
            _service.SendToNode(request, _host);
            return new byte[0];
        }
        
        public void Dispose()
        {
            // TODO
        }

        private RequestStatusCode InsertToKronosNode(CachedObject objectToCache)
        {
            _logger.Info("New request");
            
            InsertRequest request = new InsertRequest(objectToCache);
            
            _logger.Info($"Sending request to communication service");
            RequestStatusCode result = _service.SendToNode(request, _host);
            return result;
        }
    }
}
