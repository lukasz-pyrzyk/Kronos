using System;
using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using NSubstitute;
using Xunit;

namespace Kronos.Client.Tests
{
    public class KronosClientTests
    {
        [Fact]
        public async Task Insert_InsertsObjectToStorage()
        {
            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("package");
            DateTime expiryDate = DateTime.Today.AddDays(1);

            var connectionMock = Substitute.For<IConnection>();
            connectionMock.SendAsync(Arg.Any<IRequest>())
                .Returns(SerializationUtils.Serialize(true));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => connectionMock);
            await client.InsertAsync(key, package, expiryDate);

            await connectionMock.Received(1).SendAsync(Arg.Any<InsertRequest>());
        }

        [Fact]
        public async Task Get_ReturnsObject()
        {
            const string word = "lorem ipsum";
            byte[] package = SerializationUtils.Serialize(word);

            var connectionMock = Substitute.For<IConnection>();
            connectionMock.SendAsync(Arg.Any<GetRequest>())
                .Returns(SerializationUtils.Serialize(package));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => connectionMock);

            byte[] response = await client.GetAsync("key");

            string responseString = SerializationUtils.Deserialize<string>(response);
            Assert.Equal(responseString, word);
            await connectionMock.Received(1).SendAsync(Arg.Any<GetRequest>());
        }

        [Fact]
        public async Task Get_DoestNotReturnObject()
        {
            byte[] serverResponse = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            var connectionMock = Substitute.For<IConnection>();
            connectionMock.SendAsync(Arg.Any<GetRequest>())
                .Returns(SerializationUtils.Serialize(serverResponse));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => connectionMock);

            byte[] response = await client.GetAsync("key");

            Assert.Null(response);
            await connectionMock.Received(1).SendAsync(Arg.Any<GetRequest>());
        }

        [Fact]
        public async Task Delete_CallsSendToServerAsync()
        {
            var connectionMock = Substitute.For<IConnection>();
            connectionMock.SendAsync(Arg.Any<DeleteRequest>())
                .Returns(SerializationUtils.Serialize(true));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => connectionMock);

            await client.DeleteAsync("key");

            await connectionMock.Received(1).SendAsync(Arg.Any<DeleteRequest>());
        }

        [Fact]
        public async Task Count_ReturnsNumberOfElementsInStorage()
        {
            int countPerServer = 5;
            var connectionMock = Substitute.For<IConnection>();
            connectionMock.SendAsync(Arg.Any<CountRequest>())
                .Returns(SerializationUtils.Serialize(countPerServer));

            KronosConfig config = LoadTestConfiguration();
            int serverCount = config.ClusterConfig.Servers.Length;
            IKronosClient client = new KronosClient(config, endpoint => connectionMock);

            int sum = await client.CountAsync();

            Assert.Equal(sum, countPerServer * serverCount);
            await connectionMock.Received(serverCount).SendAsync(Arg.Any<CountRequest>());
        }

        [Fact]
        public async Task Contains_ReturnsTrueIfElementExistsInStorage()
        {
            bool expected = true;
            string key = "lorem ipsum";
            var connectionMock = Substitute.For<IConnection>();
            connectionMock.SendAsync(Arg.Any<ContainsRequest>())
                .Returns(SerializationUtils.Serialize(expected));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => connectionMock);

            bool exists = await client.ContainsAsync(key);

            Assert.Equal(expected, exists);
            await connectionMock.Received(1).SendAsync(Arg.Any<ContainsRequest>());
        }

        private static KronosConfig LoadTestConfiguration()
        {
            var server = new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = 5000
            };

            return new KronosConfig
            {
                ClusterConfig = new ClusterConfig
                {
                    Servers = new[] { server }
                }
            };
        }
    }
}
