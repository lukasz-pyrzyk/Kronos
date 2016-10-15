using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Configuration;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Moq;
using Newtonsoft.Json;
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
            communicationServiceMock.SendToServerAsync(It.IsAny<Request>())
                .Returns(SerializationUtils.Serialize(RequestStatusCode.Ok));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);
            client.InsertAsync(key, package, expiryDate);

            communicationServiceMock.Received(1).SendToServerAsync(It.IsAny<InsertRequest>());
        }

        [Fact]
        public async Task Get_ReturnsObject()
        {
            const string word = "lorem ipsum";
            byte[] package = SerializationUtils.Serialize(word);

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(It.IsAny<GetRequest>())
                .Returns(SerializationUtils.Serialize(package));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            byte[] response = await client.GetAsync("key");

            string responseString = SerializationUtils.Deserialize<string>(response);
            Assert.Equal(responseString, word);
            await communicationServiceMock.Received(1).SendToServerAsync(It.IsAny<GetRequest>());
        }

        [Fact]
        public async Task Get_DoestNotReturnObject()
        {
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(It.IsAny<GetRequest>())
                .Returns(SerializationUtils.Serialize(RequestStatusCode.NotFound));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            byte[] response = await client.GetAsync("key");

            Assert.Null(response);
            await communicationServiceMock.Received(1).SendToServerAsync(It.IsAny<GetRequest>());
        }

        [Fact]
        public async Task Delete_CallsSendToServerAsync()
        {
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(It.IsAny<DeleteRequest>())
                .Returns(SerializationUtils.Serialize(RequestStatusCode.Ok));

            KronosConfig config = LoadTestConfiguration();
            IKronosClient client = new KronosClient(config, endpoint => communicationServiceMock);

            await client.DeleteAsync("key");

            await communicationServiceMock.Received(1).SendToServerAsync(It.IsAny<DeleteRequest>());
        }

        private static KronosConfig LoadTestConfiguration()
        {
            var server = new ServerConfig()
            {
                Ip = "0.0.0.0",
                Port = 5000
            };

            return new KronosConfig()
            {
                ClusterConfig = new ClusterConfig()
                {
                    Servers = new ServerConfig[] { server}
                }
            };
        }
    }
}
