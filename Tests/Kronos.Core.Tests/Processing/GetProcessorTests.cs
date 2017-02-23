using System.Text;
using Kronos.Core.Processing;
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
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(expected, response.Exists);
            Assert.Equal(obj, response.Data);
        }

        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache()
        {
            // arrange
            byte[] obj = Encoding.UTF8.GetBytes("lorem ipsum");
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
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(expected, response.Exists);
            Assert.Equal(obj, response.Data);
        }
    }
}
