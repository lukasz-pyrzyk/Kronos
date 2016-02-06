using System;
using System.IO;
using Kronos.Core.Requests;
using ProtoBuf;

namespace Kronos.Server.RequestProcessing
{
    internal class RequestProcessor : IRequestProcessor
    {
        public void ProcessRequest(byte[] request)
        {
            RequestType type = (RequestType)BitConverter.ToInt16(request, 0);

            switch (type)
            {
                case RequestType.InsertRequest:
                    InsertRequest deserializedRequest = Deserialize<InsertRequest>(request);
                    break;
                case RequestType.GetRequest:
                    // TODO;
                    break;
            }
        }

        private T Deserialize<T>(byte[] request)
        {
            using (MemoryStream ms = new MemoryStream(request))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
