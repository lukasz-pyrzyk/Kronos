using System;
using System.Net;
using Kronos.Client.Transfer;
using Kronos.Core.Model;
using Kronos.Core.Requests;
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

        public void InsertToServer(string key, byte[] package, DateTime expiryDate)
        {
            _logger.Debug("New insert request");
            CachedObject objectToCache = new CachedObject(key, package, expiryDate);
            InsertRequest request = new InsertRequest(objectToCache);
            
            _service.SendToNode(request, _host);
        }

        public byte[] TryGetValue(string key)
        {
            _logger.Debug("New get request");
            GetRequest request = new GetRequest(key);

            return _service.SendToNode(request, _host);
        }
    }
}
