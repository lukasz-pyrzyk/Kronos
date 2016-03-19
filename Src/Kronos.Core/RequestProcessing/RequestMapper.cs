using System;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;

namespace Kronos.Core.RequestProcessing
{
    public class RequestMapper : IRequestMapper
    {
        public Request ProcessRequest(byte[] requestBytes, RequestType type)
        {
            Request request;
            switch (type)
            {
                case RequestType.Insert:
                    request = SerializationUtils.Deserialize<InsertRequest>(requestBytes);
                    break;
                case RequestType.Get:
                    request = SerializationUtils.Deserialize<GetRequest>(requestBytes);
                    break;
                case RequestType.Delete:
                    request = SerializationUtils.Deserialize<DeleteRequest>(requestBytes);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {type}");
            }

            return request;
        }
    }
}
