using System;
using System.IO;
using Kronos.Client.Core.Server;
using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;

namespace Kronos.Client
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly IKronosCommunicationService _service;

        public KronosClient(IKronosCommunicationService service)
        {
            _service = service;
        }

        public RequestStatusCode SaveInCache(string key, Stream stream, DateTime expiryDate)
        {
            InsertRequest request = new InsertRequest(key, stream, expiryDate);
            return SendToServer(request);
        }
        
        public RequestStatusCode SaveInCache(InsertRequest request)
        {
            return SendToServer(request);
        }

        public void Dispose()
        {
            // TODO
        }

        private RequestStatusCode SendToServer(InsertRequest request)
        {
            RequestStatusCode result = _service.SendToNode(request);
            return result;
        }
    }
}
