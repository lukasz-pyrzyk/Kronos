using Kronos.Core.Networking;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class GetProcessor : CommandProcessor<GetRequest, GetResponse>
    {
        public override GetResponse Reply(GetRequest request, IStorage storage)
        {
            //byte[] response;
            //if (!storage.TryGet(request.Key, out response))
            //{
            //    response = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            //}

            return new GetResponse(); // todo real response
        }

        protected override GetResponse ParseResponse(Response response)
        {
            return response.GetRespone;
        }
    }
}
