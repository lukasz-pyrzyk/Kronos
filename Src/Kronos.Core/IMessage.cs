using Kronos.Core.Messages;
using ZeroFormatter;

namespace Kronos.Core
{
    [Union(subTypes: new[] { typeof(ClearRequest), typeof(ContainsRequest), typeof(CountRequest), typeof(DeleteRequest), typeof(GetRequest), typeof(InsertRequest), typeof(StatsRequest) })]
    public interface IRequest
    {
        [UnionKey]
        byte Id { get; }
    }

    [Union(subTypes: new[] { typeof(ClearResponse), typeof(ContainsResponse), typeof(CountResponse), typeof(DeleteResponse), typeof(GetResponse), typeof(InsertResponse), typeof(StatsResponse) })]
    public interface IResponse
    {
        [UnionKey]
        byte Id { get; }
    }
}
