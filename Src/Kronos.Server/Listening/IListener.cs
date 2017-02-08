using System;

namespace Kronos.Server.Listening
{
    public interface IListener : IDisposable
    {
        void Start();
        void Stop();
    }
}
