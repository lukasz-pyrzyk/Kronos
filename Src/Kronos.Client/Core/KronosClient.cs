using System;
using Kronos.Client.Core.Server;
using Kronos.Shared.Configuration;
using Kronos.Shared.Network.Codes;
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
            NodeConfiguration nodeConfiguration = _configuration.GetNodeForStream(package);
            InsertRequest request = new InsertRequest(key, package, expiryDate);
            RequestStatusCode result = _service.SendToNode(request, nodeConfiguration.Endpoint);

            return result;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
