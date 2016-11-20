using Kronos.Core.Requests;
using XGain;
using XGain.Sockets;

namespace Kronos.Server.Listener
{
    public class ReceivedMessage : MessageArgs
    {
        public ReceivedMessage(ISocket client, RequestType type, byte[] buffer, int received) : base(client, buffer)
        {
            Type = type;
            Buffer = buffer;
            Received = received;
        }

        public RequestType Type { get; }
        public byte[] Buffer { get; }
        public int Received { get; }
    }
}