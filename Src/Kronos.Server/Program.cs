using Kronos.Server.Listener;

namespace Kronos.Server
{
    public class Program
    {
        private static readonly ICommunicationListener _listener = new SocketListener();

        public static void Main()
        {
            _listener.StartListening();
        }
    }
}
