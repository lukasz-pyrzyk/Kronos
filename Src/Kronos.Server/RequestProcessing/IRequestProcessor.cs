using System.Net.Sockets;

namespace Kronos.Server.RequestProcessing
{
    internal interface IRequestProcessor
    {
        void ProcessRequest(Socket client, byte[] requestBytes);
    }
}
