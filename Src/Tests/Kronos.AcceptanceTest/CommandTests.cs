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
        public async Task Insert_And_Get_WorksCorrectly()
        {
            const int port = 9999;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            byte[] received;
            var tokenSource = new CancellationTokenSource();
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    await client.InsertAsync(key, data, expiry);
                    received = await client.GetAsync(key);
                }
            }

            tokenSource.Cancel();
            Assert.Equal(data, received);
        }

        [Fact]
        public async Task Insert_And_Delete_WorksCorrectly()
        {
            const int port = 9998;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            int sizeBeforeRemoving;
            int sizeAfterRemoving;
            var tokenSource = new CancellationTokenSource();


            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    await client.InsertAsync(key, data, expiry);

                    sizeBeforeRemoving = storage.Count;

                    await client.DeleteAsync(key);

                    sizeAfterRemoving = storage.Count;
                }
            }

            tokenSource.Cancel();
            Assert.Equal(sizeBeforeRemoving, 1);
            Assert.Equal(sizeAfterRemoving, 0);
        }

        [Fact]
        public async Task Insert_And_Count_WorksCorrectly()
        {
            const int port = 9998;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            var tokenSource = new CancellationTokenSource();

            int countFromClientApi;
            int countFromStorage;
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    await client.InsertAsync(key, data, expiry);

                    countFromClientApi = await client.CountAsync();
                    countFromStorage = storage.Count;
                }
            }

            tokenSource.Cancel();
            Assert.Equal(countFromClientApi, 1);
            Assert.Equal(countFromClientApi, countFromStorage);
        }

        [Fact]
        public async Task Insert_And_Contains_WorksCorrectly()
        {
            const int port = 9998;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            var tokenSource = new CancellationTokenSource();

            bool containsFromClientApi;
            bool containsFromStorage;
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    await client.InsertAsync(key, data, expiry);

                    containsFromClientApi = await client.ContainsAsync(key);
                    containsFromStorage = storage.Contains(key);
                }
            }

            tokenSource.Cancel();
            Assert.True(containsFromClientApi);
            Assert.Equal(containsFromClientApi, containsFromStorage);
        }

        [Fact]
        public async Task Contains_WorksCorrectly()
        {
            const int port = 9998;
            const string key = "key";

            var tokenSource = new CancellationTokenSource();

            bool containsFromClientApi;
            bool containsFromStorage;
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    worker.StartListeningAsync(tokenSource.Token);

                    IKronosClient client = KronosClientFactory.CreateClient(port);

                    containsFromClientApi = await client.ContainsAsync(key);
                    containsFromStorage = storage.Contains(key);
                }
            }

            tokenSource.Cancel();
            Assert.False(containsFromClientApi);
            Assert.Equal(containsFromClientApi, containsFromStorage);
        }
    }
}
