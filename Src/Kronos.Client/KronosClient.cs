using System;
using System.Diagnostics;
using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Model;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;

namespace Kronos.Client
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    internal class KronosClient : IKronosClient
    {
        private readonly IClientServerConnection _service;

        public KronosClient(IClientServerConnection service)
        {
            _service = service;
        }

        public void InsertToServer(string key, byte[] package, DateTime expiryDate)
        {
            Trace.WriteLine("New insert request");
            CachedObject objectToCache = new CachedObject(key, package, expiryDate);
            InsertRequest request = new InsertRequest(objectToCache);
            InsertCommand command = new InsertCommand(_service, request);

            RequestStatusCode status = command.Execute();

            Trace.WriteLine($"InsertRequest status: {status}");
        }

        public byte[] TryGetValue(string key)
        {
            Trace.WriteLine("New get request");
            GetRequest request = new GetRequest(key);
            GetCommand command = new GetCommand(_service, request);

            byte[] valueFromCache = command.Execute();

            Trace.WriteLine($"GetRequest status returned object with {valueFromCache.Length} bytes");

            return valueFromCache;
        }
    }
}
