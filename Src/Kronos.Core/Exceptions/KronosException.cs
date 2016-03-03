using System;

namespace Kronos.Core.Model.Exceptions
{
    public class KronosException : Exception
    {
        public KronosException()
        {
        }

        public KronosException(string message)
            : base(message)
        {
        }

        public KronosException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
