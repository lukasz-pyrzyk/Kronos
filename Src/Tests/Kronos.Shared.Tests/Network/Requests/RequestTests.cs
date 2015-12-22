using System.Net;
using Kronos.Shared.Network.Requests;
using Xunit;

namespace Kronos.Shared.Tests.Network.Requests
{
    public class RequestTests
    {
        [Theory]
        [InlineData("8.8.8.8", 7)]
        [InlineData("8.8.8.4", 8)]
        public void CanAssignPropertiesByConstructor(string host, int port)
        {
            Request request = new Request(host, port);

            Assert.Equal(host, request.Host);
            Assert.Equal(port, request.Port);
        }

        [Theory]
        [InlineData("8.8.8.8", 7)]
        [InlineData("8.8.8.4", 8)]
        public void ReturnsGoodIPEndpoint(string host, int port)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
            Request request = new Request(host, port);

            Assert.Equal(endpoint, request.Endpoint);
        }

        [Theory]
        [InlineData("8.8.8.8", 7)]
        [InlineData("8.8.8.4", 8)]
        public void ReturnsIpEndpointInToStringMethod(string host, int port)
        {
            Request request = new Request(host, port);
            string message = request.ToString();

            Assert.Equal(message, $"{host}:{port}");
        }
    }
}
