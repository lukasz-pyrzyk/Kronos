using System;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Listening
{
    public interface IListener : IDisposable
    {
        void Start();
        void Stop();
    }
}
