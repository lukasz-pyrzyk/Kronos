using System;
using System.Buffers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;
using Kronos.Core.Serialization;

namespace Kronos.Server
{
    public class SocketProcessor : ISocketProcessor
    {
        public Request ReceiveRequest(Socket client, Memory<byte> buffer)
        {
            SocketUtils.ReceiveAll(client, buffer);
            var request = new Request();
            var stream = new DeserializationStream(buffer);
            request.Read(ref stream);
            return request;
        }

        public void SendResponse(Socket client, Response response, Memory<byte> buffer)
        {
            var stream = new SerializationStream(buffer);
            response.Write(ref stream);
            stream.Flush();
            SocketUtils.SendAll(client, stream.MemoryWithLengthPrefix.Span);
        }
    }
}
