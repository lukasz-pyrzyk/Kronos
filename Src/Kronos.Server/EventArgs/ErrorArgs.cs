using System;

namespace Kronos.Server.EventArgs
{
    public class ErrorArgs : System.EventArgs
    {
        public ErrorArgs(Exception ex)
        {
            Date = DateTime.Now;
            Exception = ex;
        }

        public DateTime Date { get; }
        public Exception Exception { get; }
    }
}
