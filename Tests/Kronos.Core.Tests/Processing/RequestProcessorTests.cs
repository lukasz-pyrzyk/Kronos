using System;
using FluentAssertions;
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
            var request = new Request { Auth = Auth.FromCfg(DefaultSettings.Login, DefaultSettings.HashedPassword) };
            var serverAuth = Auth.Default();

            // Act
            Response response = processor.Handle(request, serverAuth);

            // Assert
            AssertFailure(response);
            response.Exception.Should().Contain(request.Auth.Login);
        }

        [Fact]
        public void Handle_InvalidRequest_ReturnsException()
        {
            // Arrange
            var processor = new RequestProcessor(Substitute.For<IStorage>(), null, null, null, null, null, null, null);
            var request = new Request { Auth = Auth.Default(), Type = RequestType.Unknown };

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertFailure(response);
            response.Exception.Should().Contain(request.Type.ToString());
        }

        private static void AssertFailure(Response response)
        {
            response.Success.Should().BeFalse();
            response.Exception.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Handle_Insert_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), insertProcessor: new InsertProcessor());
            var request = InsertRequest.New("key", new byte[2014], DateTime.UtcNow, Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.InsertRequest.Should().NotBeNull();
        }

        [Fact]
        public void Handle_Get_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), getProcessor: new GetProcessor());
            var request = GetRequest.New("key", Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.GetRequest.Should().NotBeNull();
        }

        [Fact]
        public void Handle_Delete_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), deleteProcessor: new DeleteProcessor());
            var request = DeleteRequest.New("key", Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.DeleteRequest.Should().NotBeNull();
        }

        [Fact]
        public void Handle_Count_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), countProcessor: new CountProcessor());
            var request = CountRequest.New(Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.CountRequest.Should().NotBeNull();
        }

        [Fact]
        public void Handle_Contains_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), containsProcessor: new ContainsProcessor());
            var request = ContainsRequest.New("key", Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.ContainsRequest.Should().NotBeNull();
        }

        [Fact]
        public void Handle_Clear_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), clearProcessor: new ClearProcessor());
            var request = ClearRequest.New(Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.ClearRequest.Should().NotBeNull();
        }

        [Fact]
        public void Handle_Stats_WorksCorrectly()
        {
            // Arrange
            var processor = CreateProcessor(Substitute.For<IStorage>(), statsProcessor: new StatsProcessor());
            var request = StatsRequest.New(Auth.Default());

            // Act
            Response response = processor.Handle(request, Auth.Default());

            // Assert
            AssertSuccessful(response);
            request.StatsRequest.Should().NotBeNull();
        }

        private static void AssertSuccessful(Response response)
        {
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Exception.Should().BeEmpty();
        }

        private static RequestProcessor CreateProcessor(IStorage storage,
            CommandProcessor<InsertRequest, InsertResponse> insertProcessor = null,
            CommandProcessor<GetRequest, GetResponse> getProcessor = null,
            CommandProcessor<DeleteRequest, DeleteResponse> deleteProcessor = null,
            CommandProcessor<CountRequest, CountResponse> countProcessor = null,
            CommandProcessor<ContainsRequest, ContainsResponse> containsProcessor = null,
            CommandProcessor<ClearRequest, ClearResponse> clearProcessor = null,
            CommandProcessor<StatsRequest, StatsResponse> statsProcessor = null)
        {
            return new RequestProcessor(storage, insertProcessor, getProcessor, deleteProcessor, countProcessor, containsProcessor, clearProcessor, statsProcessor);
        }
    }
}
