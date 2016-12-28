using System.Text;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class GetProcessorTests
    {
        public void Handle_ReturnsObjectFromCache()
        {
            // arrange
            byte[] obj = Encoding.UTF8.GetBytes("lorem ipsum");
            bool expected = true;
            byte[] dummy;
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out dummy).Returns(x =>
            {
                x[1] = obj;
                return expected;
            });

            // Act
            byte[] response = processor.Process(ref request, storage);

            // obj
            Assert.Equal(obj, response);
        }

        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache()
        {
            // arrange
            byte[] obj = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            byte[] dummy;
            bool expected = false;
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out dummy).Returns(x =>
            {
                x[1] = obj;
                return expected;
            });

            // Act
            byte[] response = processor.Process(ref request, storage);

            // assert
            Assert.Equal(obj, response);
        }
    }
}
