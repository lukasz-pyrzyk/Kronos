using System;
using System.Diagnostics;
using System.Net;
using Kronos.Core.Communication;
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
            InsertRequest request = new InsertRequest(key, package, expiryDate);
            RequestStatusCode status = request.Execute<RequestStatusCode>(_service);

            Trace.WriteLine($"InsertRequest status: {status}");
        }

        public byte[] TryGetValue(string key)
        {
            Trace.WriteLine("New get request");
            GetRequest request = new GetRequest(key);

            byte[] valueFromCache = request.Execute<byte[]>(_service);

            if (valueFromCache != null && valueFromCache.Length == 1 && valueFromCache[0] == 0)
                return null;

            return valueFromCache;
        }

        public void TryDelete(string key)
        {
            Trace.WriteLine("New delete request");

            DeleteRequest request = new DeleteRequest(key);
            RequestStatusCode status = request.Execute<RequestStatusCode>(_service);

            Trace.WriteLine($"InsertRequest status: {status}");
        }
    }
}
