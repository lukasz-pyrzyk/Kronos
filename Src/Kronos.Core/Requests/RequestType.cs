using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public enum RequestType : short
    {
        [ProtoEnum]
        Insert = 1,

        [ProtoEnum]
        Get = 2,

        [ProtoEnum]
        Delete = 3
    }
}
