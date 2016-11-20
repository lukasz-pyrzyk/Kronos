using System;
using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Network;
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

            var communicationServiceMock = Substitute.For<IConnection>();
            communicationServiceMock.Send(Arg.Any<IRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(true));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);
            await client.InsertAsync(key, package, expiryDate);

            communicationServiceMock.Received(1).Send(Arg.Any<InsertRequest>());
        }

        [Fact]
        public async Task Get_ReturnsObject()
        {
            const string word = "lorem ipsum";
            byte[] package = SerializationUtils.Serialize(word);

            var communicationServiceMock = Substitute.For<IConnection>();
            communicationServiceMock.Send(Arg.Any<GetRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(package));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            byte[] response = await client.GetAsync("key");

            string responseString = SerializationUtils.Deserialize<string>(response);
            Assert.Equal(responseString, word);
            communicationServiceMock.Received(1).Send(Arg.Any<GetRequest>());
        }

        [Fact]
        public async Task Get_DoestNotReturnObject()
        {
            byte[] serverResponse = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            var communicationServiceMock = Substitute.For<IConnection>();
            communicationServiceMock.Send(Arg.Any<GetRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(serverResponse));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            byte[] response = await client.GetAsync("key");

            Assert.Null(response);
            communicationServiceMock.Received(1).Send(Arg.Any<GetRequest>());
        }

        [Fact]
        public async Task Delete_CallsSendToServerAsync()
        {
            var communicationServiceMock = Substitute.For<IConnection>();
            communicationServiceMock.Send(Arg.Any<DeleteRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(true));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            await client.DeleteAsync("key");

            communicationServiceMock.Received(1).Send(Arg.Any<DeleteRequest>());
        }

        [Fact]
        public async Task Count_ReturnsNumberOfElementsInStorage()
        {
            int countPerServer = 5;
            var communicationServiceMock = Substitute.For<IConnection>();
            communicationServiceMock.Send(Arg.Any<CountRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(countPerServer));

            KronosConfig config = LoadTestConfiguration();
            int serverCount = config.ClusterConfig.Servers.Length;
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            int sum = await client.CountAsync();

            Assert.Equal(sum, countPerServer * serverCount);
            communicationServiceMock.Received(serverCount).Send(Arg.Any<CountRequest>());
        }

        [Fact]
        public async Task Contains_ReturnsTrueIfElementExistsInStorage()
        {
            bool expected = true;
            string key = "lorem ipsum";
            var communicationServiceMock = Substitute.For<IConnection>();
            communicationServiceMock.Send(Arg.Any<ContainsRequest>())
                .Returns(SerializationUtils.SerializeToStreamWithLength(expected));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            bool exists = await client.ContainsAsync(key);

            Assert.Equal(expected, exists);
            communicationServiceMock.Received(1).Send(Arg.Any<ContainsRequest>());
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
