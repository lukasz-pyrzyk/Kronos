using System;
using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Configuration;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using NSubstitute;
using Xunit;

namespace Kronos.Client.Tests
{
    public class KronosClientTests
    {
        [Fact]
        public void Insert_InsertsObjectToStorage()
        {
            string key = "key";
            byte[] package = Encoding.UTF8.GetBytes("package");
            DateTime expiryDate = DateTime.Today.AddDays(1);

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(Arg.Any<Request>())
                .Returns(SerializationUtils.Serialize(RequestStatusCode.Ok));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);
            client.InsertAsync(key, package, expiryDate);

            communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<InsertRequest>());
        }

        [Fact]
        public async Task Get_ReturnsObject()
        {
            const string word = "lorem ipsum";
            byte[] package = SerializationUtils.Serialize(word);

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(Arg.Any<GetRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(package));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            byte[] response = await client.GetAsync("key");

            string responseString = SerializationUtils.Deserialize<string>(response);
            Assert.Equal(responseString, word);
            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<GetRequest>());
        }

        [Fact]
        public async Task Get_DoestNotReturnObject()
        {
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(Arg.Any<GetRequest>())
                .Returns(SerializationUtils.Serialize(RequestStatusCode.NotFound));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            byte[] response = await client.GetAsync("key");

            Assert.Null(response);
            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<GetRequest>());
        }

        [Fact]
        public async Task Delete_CallsSendToServerAsync()
        {
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(Arg.Any<DeleteRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(RequestStatusCode.Ok));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            await client.DeleteAsync("key");

            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<DeleteRequest>());
        }

        [Fact]
        public async Task Count_ReturnsNumberOfElementsInStorage()
        {
            int countPerServer = 5;
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(Arg.Any<CountRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(countPerServer));

            KronosConfig config = LoadTestConfiguration();
            int serverCount = config.ClusterConfig.Servers.Length;
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            int sum = await client.CountAsync();

            Assert.Equal(sum, countPerServer * serverCount);
            await communicationServiceMock.Received(serverCount).SendToServerAsync(Arg.Any<CountRequest>());
        }

        [Fact]
        public async Task Contains_ReturnsTrueIfElementExistsInStorage()
        {
            bool expected = true;
            string key = "lorem ipsum";
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(Arg.Any<ContainsRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(expected));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            bool exists = await client.ContainsAsync(key);

            Assert.Equal(expected, exists);
            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<ContainsRequest>());
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
