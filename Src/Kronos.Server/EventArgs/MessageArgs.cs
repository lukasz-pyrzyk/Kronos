using System.Net.Sockets;

namespace Kronos.Server.EventArgs
{
    public class MessageArgs : System.EventArgs
    {
        public MessageArgs(Socket client, byte[] requestBytes, object userToken = null)
        {
            Client = client;
            RequestBytes = requestBytes;
            UserToken = userToken;
        }

        public Socket Client { get; }
        public byte[] RequestBytes { get; }
        public object UserToken { get; }
    }
}
