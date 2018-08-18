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
            request.Read(new DeserializationStream(buffer));
            return request;
        }

        public void SendResponse(Socket client, Response response)
        {
            int neededSize = response.CalculateSize();
            using (var stream = new SerializationStream(neededSize))
            {
                response.Write(stream);
                stream.Flush();

                SocketUtils.SendAll(client, stream.MemoryWithLengthPrefix.Span);
            }
        }
    }
}
