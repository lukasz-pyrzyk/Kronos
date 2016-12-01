using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Listening
{
    public class Listener<TMessage> : IListener where TMessage : RequestArgs
    {
        public event EventHandler<StartArgs> OnStart;
        public event EventHandler<RequestArgs> OnNewMessage;
        public event EventHandler<ErrorArgs> OnError;

        private readonly TcpListener _listener;
        private readonly IProcessor<TMessage> _processor;

        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        public Listener(IPAddress ipAddress, int port, IProcessor<TMessage> processor)
        {
            _listener = new TcpListener(ipAddress, port);
            _processor = processor;
        }

        public void Start(int? maxDegreeOfParallelism = null)
        {
            _listener.Start();
            RaiseOnStartEvent();

            CancellationToken token = _cancel.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        break;

                    try
                    {
                        Socket socket = await _listener.AcceptSocketAsync();
                        ProcessSocketConnection(socket);
                    }
                    catch (Exception ex)
                    {
                        RaiseOnError(ex);
                    }
                }
            }, token);
        }

        public void Stop()
        {
            try
            {
                _cancel.Cancel();
                _listener.Stop();
            }
            catch (SocketException ex)
            {
                RaiseOnError(ex);
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private async void ProcessSocketConnection(Socket socket)
        {
            RequestArgs args = await _processor.ProcessSocketConnectionAsync(socket);
            RaiseOnNewMessageEvent(socket, args);
        }

        private void RaiseOnStartEvent()
        {
            OnStart?.Invoke(this, new StartArgs(_listener.LocalEndpoint));
        }

        private void RaiseOnError(Exception ex)
        {
            OnError?.Invoke(this, new ErrorArgs(ex));
        }

        private void RaiseOnNewMessageEvent(Socket socket, RequestArgs args)
        {
            OnNewMessage?.Invoke(socket, args);
        }
    }
}
