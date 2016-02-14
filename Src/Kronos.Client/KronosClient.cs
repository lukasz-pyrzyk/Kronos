using System;
using Kronos.Client.Command;
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

        public KronosClient(ICommunicationService service)
        {
            _service = service;
        }

        public void InsertToServer(string key, byte[] package, DateTime expiryDate)
        {
            _logger.Debug("New insert request");
            CachedObject objectToCache = new CachedObject(key, package, expiryDate);
            InsertRequest request = new InsertRequest(objectToCache);
            InsertCommand command = new InsertCommand(_service, request);

            RequestStatusCode status = command.Execute();
           
            _logger.Debug($"InsertRequest status: {status}");
        }

        public byte[] TryGetValue(string key)
        {
            _logger.Debug("New get request");
            GetRequest request = new GetRequest(key);
            GetCommand command = new GetCommand(_service, request);

            byte[] valueFromCache = command.Execute();

            _logger.Debug($"GetRequest status returned object with {valueFromCache.Length} bytes");

            return valueFromCache;
        }
    }
}
