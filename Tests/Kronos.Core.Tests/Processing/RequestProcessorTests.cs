using System;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class RequestProcessorTests
    {
        [Fact]
        public void Handle_ReturnsInformationAboutUnauthorized()
        {
            // Arrange
            IStorage storage = Substitute.For<IStorage>();
            var processor = new RequestProcessor(storage);
            var request = new Request { Auth = Auth.FromCfg(new AuthConfig { Login = "random", Password = "random" }) };
            var serverAuth = Auth.Default();

            // Act
            Response response = processor.Handle(request, serverAuth);

            // Assert
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
            Assert.Contains(request.Auth.Login, response.Exception);
        }

        [Fact]
        public void Handle_Insert_WorksCorrectly()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), new InsertProcessor(), null, null, null, null, null);
            var request = InsertRequest.New("key", new byte[2014], DateTime.UtcNow, Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.True(response.Success);
            Assert.True(string.IsNullOrEmpty(response.Exception));
            Assert.NotNull(request.InsertRequest);
        }

        [Fact]
        public void Handle_Get_WorksCorrectly()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, new GetProcessor(), null, null, null, null);
            var request = GetRequest.New("key", Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.True(response.Success);
            Assert.True(string.IsNullOrEmpty(response.Exception));
            Assert.NotNull(request.GetRequest);
        }

        [Fact]
        public void Handle_Delete_WorksCorrectly()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, null, new DeleteProcessor(), null, null, null);
            var request = DeleteRequest.New("key", Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.True(response.Success);
            Assert.True(string.IsNullOrEmpty(response.Exception));
            Assert.NotNull(request.DeleteRequest);
        }

        [Fact]
        public void Handle_Count_WorksCorrectly()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, null, null, new CountProcessor(), null, null);
            var request = CountRequest.New(Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.True(response.Success);
            Assert.True(string.IsNullOrEmpty(response.Exception));
            Assert.NotNull(request.CountRequest);
        }

        [Fact]
        public void Handle_Contains_WorksCorrectly()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, null, null, null, new ContainsProcessor(), null);
            var request = ContainsRequest.New("key", Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.True(response.Success);
            Assert.True(string.IsNullOrEmpty(response.Exception));
            Assert.NotNull(request.ContainsRequest);
        }

        [Fact]
        public void Handle_Clear_WorksCorrectly()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, null, null, null, null, new ClearProcessor());
            var request = ClearRequest.New(Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.True(response.Success);
            Assert.True(string.IsNullOrEmpty(response.Exception));
            Assert.NotNull(request.ClearRequest);
        }

        [Fact]
        public void Handle_InvalidRequest_ReturnsException()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, null, null, null, null, null);
            var request = new Request { Auth = Auth.Default(), Type = RequestType.Unknown };

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            Assert.False(response.Success);
            Assert.Contains(request.Type.ToString(), response.Exception);
        }
    }
}
