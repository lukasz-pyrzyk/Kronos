﻿using System;
using System.IO;
using Kronos.Client.Core.Server;
using Kronos.Shared.Configuration;
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
        private readonly ICommunicationService _service;
        private readonly IServerConfiguration _configuration;

        public KronosClient(ICommunicationService service, IServerConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        public RequestStatusCode InsertToServer(string key, Stream stream, DateTime expiryDate)
        {
            NodeConfiguration nodeConfiguration = _configuration.GetNodeForStream(stream);
            InsertRequest request = new InsertRequest(key, stream, expiryDate, nodeConfiguration.Host, nodeConfiguration.Port);
            RequestStatusCode result = _service.SendToNode(request);

            return result;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
