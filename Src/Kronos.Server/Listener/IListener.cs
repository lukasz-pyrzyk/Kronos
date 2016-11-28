using System;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Listener
{
    public interface IListener : IDisposable
    {
        event EventHandler<StartArgs> OnStart;
        event EventHandler<RequestArgs> OnNewMessage;
        event EventHandler<ErrorArgs> OnError;

        void Start(int? maxDegreeOfParallelism = null);
        void Stop();
    }
}
