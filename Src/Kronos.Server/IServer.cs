using System;
using Kronos.Server.EventArgs;

namespace Kronos.Server
{
    public interface IServer : IDisposable
    {
        event EventHandler<StartArgs> OnStart;
        event EventHandler<MessageArgs> OnNewMessage;
        event EventHandler<ErrorArgs> OnError;

        void Start(int? maxDegreeOfParallelism = null);
        void Stop();
    }
}
