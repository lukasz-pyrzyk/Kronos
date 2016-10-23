using System;
using System.Collections.Generic;
using Kronos.Core.Serialization;

namespace Kronos.Core.Requests
{
    public class RequestMapper : IRequestMapper
    {
        public static Dictionary<RequestType, Type> Mapping = new Dictionary<RequestType, Type>()
        {
            [RequestType.Insert] = typeof(InsertRequest),
            [RequestType.Get] = typeof(GetRequest),
            [RequestType.Contains] = typeof(ContainsRequest),
            [RequestType.Count] = typeof(CountRequest),
            [RequestType.Delete] = typeof(DeleteRequest)
        };

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
                case RequestType.Count:
                    request = SerializationUtils.Deserialize<CountRequest>(requestBytes);
                    break;
                case RequestType.Contains:
                    request = SerializationUtils.Deserialize<ContainsRequest>(requestBytes);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {type}");
            }

            return request;
        }
    }
}
