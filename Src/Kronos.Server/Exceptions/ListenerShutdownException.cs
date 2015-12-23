using System;

namespace Kronos.Server.Exceptions
{
    public class ListenerShutdownException : Exception
    {
        public ListenerShutdownException()
        {
        }

        public ListenerShutdownException(string message)
        : base(message)
        {
        }

        public ListenerShutdownException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
