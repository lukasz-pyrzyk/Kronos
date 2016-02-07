using System;
using System.IO;
using System.Linq;
using Kronos.Core.Requests;
using ProtoBuf;

namespace Kronos.Server.RequestProcessing
{
    internal class RequestProcessor : IRequestProcessor
    {
        public void ProcessRequest(byte[] request)
        {
            RequestType type = Deserialize<RequestType>(request.Skip(sizeof (short)).ToArray());

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
