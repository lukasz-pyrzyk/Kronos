using System.IO;
using Kronos.Core.Requests;
using Kronos.Server.Storage;
using ProtoBuf;

namespace Kronos.Server.RequestProcessing
{
    internal class RequestProcessor : IRequestProcessor
    {
        public void ProcessRequest(byte[] request)
        {
            InsertRequest insertRequest;
            using (MemoryStream ms = new MemoryStream(request))
            {
                insertRequest = Serializer.Deserialize<InsertRequest>(ms);
            }

            InMemoryStorage.AddOrUpdate(insertRequest.ObjectToCache.Key, insertRequest.ObjectToCache.Object);
        }
    }
}
