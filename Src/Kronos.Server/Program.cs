using System;
using Kronos.Server.Listener;

namespace Kronos.Server
{
    public class Program
    {
        private static readonly ICommunicationListener _listener = new SocketListener();

        public static void Main()
        {
            Console.WriteLine("Starting server");

            _listener.StartListening();

            Console.WriteLine("Stopping server");
        }
    }
}
