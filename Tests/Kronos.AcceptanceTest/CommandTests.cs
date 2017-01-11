using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Kronos.Server.EventArgs;
using Kronos.Server.Listening;
using Xunit;

namespace Kronos.AcceptanceTest
{
    public class CommandTests
    {
        private const string localHost = "localhost";

        [Fact]
        public async Task Insert_And_Get_WorksCorrectly()
        {
            const int port = 9999;
            const string key = "key";

            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            byte[] received;

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                var processor = new SocketProcessor();
                var requestProcessor = new RequestProcessor(storage);
                using (IListener server = new Listener(IPAddress.Any, port, processor, requestProcessor))
                {
                    server.Start();
                    IKronosClient client = KronosClientFactory.FromDomain(localHost, port);
                    await client.InsertAsync(key, data, expiry);
                    received = await client.GetAsync(key);
                }
            }

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

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                var processor = new SocketProcessor();
                var requestProcessor = new RequestProcessor(storage);
                using (IListener server = new Listener(IPAddress.Any, port, processor, requestProcessor))
                {
                    server.Start();
                    IKronosClient client = KronosClientFactory.FromDomain(localHost, port);
                    await client.InsertAsync(key, data, expiry);

                    sizeBeforeRemoving = storage.Count;

                    await client.DeleteAsync(key);

                    sizeAfterRemoving = storage.Count;
                }
            }

            Assert.Equal(sizeBeforeRemoving, 1);
            Assert.Equal(sizeAfterRemoving, 0);
        }

        [Fact]
        public async Task Insert_And_Count_WorksCorrectly()
        {
            const int port = 9997;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            int countFromClientApi;
            int countFromStorage;

            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                var processor = new SocketProcessor();
                var requestProcessor = new RequestProcessor(storage);
                using (IListener server = new Listener(IPAddress.Any, port, processor, requestProcessor))
                {
                    server.Start();
                    IKronosClient client = KronosClientFactory.FromDomain(localHost, port);
                    await client.InsertAsync(key, data, expiry);

                    countFromClientApi = await client.CountAsync();
                    countFromStorage = storage.Count;
                }
            }

            Assert.Equal(countFromClientApi, 1);
            Assert.Equal(countFromClientApi, countFromStorage);
        }

        [Fact]
        public async Task Insert_And_Contains_WorksCorrectly()
        {
            const int port = 9996;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            bool containsFromClientApi;
            bool containsFromStorage;
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                var processor = new SocketProcessor();
                var requestProcessor = new RequestProcessor(storage);
                using (IListener server = new Listener(IPAddress.Any, port, processor, requestProcessor))
                {
                    server.Start();
                    IKronosClient client = KronosClientFactory.FromDomain(localHost, port);
                    await client.InsertAsync(key, data, expiry);

                    containsFromClientApi = await client.ContainsAsync(key);
                    containsFromStorage = storage.Contains(key);
                }
            }

            Assert.True(containsFromClientApi);
            Assert.Equal(containsFromClientApi, containsFromStorage);
        }

        [Fact]
        public async Task Contains_WorksCorrectly()
        {
            const int port = 9995;
            const string key = "key";

            bool containsFromClientApi;
            bool containsFromStorage;
            IExpiryProvider expiryProvider = new StorageExpiryProvider();
            using (IStorage storage = new InMemoryStorage(expiryProvider))
            {
                var processor = new SocketProcessor();
                var requestProcessor = new RequestProcessor(storage);
                using (IListener server = new Listener(IPAddress.Any, port, processor, requestProcessor))
                {
                    server.Start();
                    IKronosClient client = KronosClientFactory.FromDomain(localHost, port);

                    containsFromClientApi = await client.ContainsAsync(key);
                    containsFromStorage = storage.Contains(key);
                }
            }

            Assert.False(containsFromClientApi);
            Assert.Equal(containsFromClientApi, containsFromStorage);
        }
    }
}
