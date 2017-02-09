using System.Buffers;
using System.Net.Sockets;

namespace Kronos.Server.Listening
{
    public interface IProcessor
    {
        void ReceiveRequest(Socket client, ref RequestArg args, ArrayPool<byte> pool);
    }
}
