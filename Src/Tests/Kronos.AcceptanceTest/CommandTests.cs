using System;
using System.Net;
using System.Text;
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
            const int port = 5000;
            const string key = "key";
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiry = DateTime.MaxValue;

            byte[] received;
            using (IStorage storage = new InMemoryStorage())
            {
                IProcessor<MessageArgs> processor = new SocketProcessor();
                using (IServer server = new XGainServer(IPAddress.Any, port, processor))
                {
                    IRequestMapper mapper = new RequestMapper();
                    IServerWorker worker = new ServerWorker(mapper, storage, server);
                    Task.Run(() => worker.StartListening());

                    IKronosClient client = KronosClientFactory.CreateClient(port);
                    client.Insert(key, data, expiry);
                    received = client.Get(key);
                }
            }

            Assert.Equal(data, received);
        }
    }
}
