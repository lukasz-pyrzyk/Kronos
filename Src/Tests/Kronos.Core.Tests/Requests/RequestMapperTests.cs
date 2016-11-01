using System;
using System.Collections.Generic;
using Kronos.Core.Processors;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestMapperTests
    {
        [Fact]
        public void ProcessRequest_ThrowsWhenProcessorForRequestTypeIsNotDefined()
        {
            byte[] requestBytes = new byte[0];
            RequestType type = RequestType.Unknown;
            IStorage storage = Substitute.For<IStorage>();

            IRequestProcessor processor = new RequestProcessor(storage);

            Exception ex =
                Assert.Throws<InvalidOperationException>(
                    () => processor.HandleIncomingRequest(type, requestBytes, Substitute.For<ISocket>()));

            Assert.Equal(ex.Message, $"Cannot find processor for type {type}");
        }

        [Theory(Skip = "Fix it")]
        [MemberData(nameof(TestData))]
        public void ProcessRequest_DetectsAndDeserializesReqeustType(IRequest request, object specificProcessor)
        {
            byte[] requestBytes = SerializationUtils.Serialize(request);
            IStorage storage = Substitute.For<IStorage>();
            ISocket socket = Substitute.For<ISocket>();

            IRequestProcessor processor = new RequestProcessor(storage);

            processor.HandleIncomingRequest(request.Type, requestBytes, socket);
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { new InsertRequest(), new InsertProcessor() };
            yield return new object[] { new GetRequest(), new GetProcessor()};
            yield return new object[] { new DeleteRequest(), new DeleteProcessor() };
            yield return new object[] { new CountRequest(), new CountProcessor() };
            yield return new object[] { new ContainsRequest(), new ContainsProcessor()};
        }
    }
}
