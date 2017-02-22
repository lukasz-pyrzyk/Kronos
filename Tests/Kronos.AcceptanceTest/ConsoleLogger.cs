using System;
using System.Diagnostics;

namespace Kronos.AcceptanceTest
{
    public class ConsoleLogger : TraceListener
    {
        public override void Write(string message)
        {
            Console.Write(FormatMessage(message));
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(FormatMessage(message));
        }

        private static string FormatMessage(string message)
        {
            return $"{DateTime.Now} - {message}";
        }
    }
}
