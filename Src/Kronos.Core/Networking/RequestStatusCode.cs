using ProtoBuf;

namespace Kronos.Core.Networking
{
    [ProtoContract]
    public enum RequestStatusCode : ushort
    {
        [ProtoEnum]
        Unknown = 0,

        [ProtoEnum]
        Ok = 1,

        [ProtoEnum]
        Failed = 2,

        [ProtoEnum]
        NotFound = 3,

        [ProtoEnum]
        Deleted = 4
    }
}
