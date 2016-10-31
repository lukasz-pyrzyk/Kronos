using Kronos.Core.Requests;

namespace Kronos.Server.Listener
{
    internal struct ReceivedMessage
    {
        public ReceivedMessage(RequestType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        public RequestType Type { get; }
        public byte[] Data { get; }
    }
}