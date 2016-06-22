using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using XGain;
using XGain.Processing;
using Xunit;

namespace Kronos.AcceptanceTest
{
    public class CommandTests
    {
        [Fact]
        public void Insert_And_Get_WorksCorrectly()
        {
            const int port = 9999;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            byte[] received;
            var tokenSource = new CancellationTokenSource();
            using (IStorage storage = new InMemoryStorage())
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    client.Insert(key, data, expiry);
                    received = client.Get(key);
                }
            }

            tokenSource.Cancel();
            Assert.Equal(data, received);
        }

        [Fact]
        public void Insert_And_Delete_WorksCorrectly()
        {
            const int port = 9998;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            int sizeBeforeRemoving;
            int sizeAfterRemoving;
            var tokenSource = new CancellationTokenSource();
            Task workerTask;
            using (IStorage storage = new InMemoryStorage())
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    workerTask = worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    client.Insert(key, data, expiry);

                    sizeBeforeRemoving = storage.Count;

                    client.Delete(key);

                    sizeAfterRemoving = storage.Count;
                }
            }

            tokenSource.Cancel();
            Assert.Equal(sizeBeforeRemoving, 1);
            Assert.Equal(sizeAfterRemoving, 0);
        }
    }
}
