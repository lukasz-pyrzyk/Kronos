using System;

namespace Kronos.Server
{
    public interface IListener : IDisposable
    {
        void Start();
        void Stop();
    }
}
