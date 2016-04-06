using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public enum RequestType : ushort
    {
        [ProtoEnum]
        Unknown = 0,

        [ProtoEnum]
        Insert = 1,

        [ProtoEnum]
        Get = 2,

        [ProtoEnum]
        Delete = 3
    }
}
