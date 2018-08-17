using Kronos.Core.Serialization;

namespace Kronos.Core
{
    public interface IRequest : ISerializable<object>
    {
    }

    public interface IResponse : ISerializable<object>
    {
    }
}
