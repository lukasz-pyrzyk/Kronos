using System;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Xunit;

namespace Kronos.Core.Tests.RequestProcessing
{
    public class RequestProcessorTests
    {
        [Fact]
        public void ProcessRequest_ThrowsWhenProcessorForRequestTypeIsNotDefined()
        {
            byte[] requestBytes = new byte[0];
            RequestType type = RequestType.Unknown;

            IRequestMapper processor = new RequestMapper();

            Exception ex =
                Assert.Throws<InvalidOperationException>(
                    () => processor.ProcessRequest(requestBytes, type));

            Assert.Equal(ex.Message, $"Cannot find processor for type {type}");
        }

        [Fact]
        public void ProcessRequest_DetectsAndDeserializesReqeustType_Insert()
        {
            var request = new InsertRequest();
            byte[] requestBytes = SerializationUtils.Serialize(request);

            IRequestMapper processor = new RequestMapper();

            Request mappedRequest = processor.ProcessRequest(requestBytes, request.RequestType);
            Assert.Equal(mappedRequest.GetType(), request.GetType());
        }

        [Fact]
        public void ProcessRequest_DetectsAndDeserializesReqeustType_Get()
        {
            var request = new GetRequest();
            byte[] requestBytes = SerializationUtils.Serialize(request);

            IRequestMapper processor = new RequestMapper();

            Request mappedRequest = processor.ProcessRequest(requestBytes, request.RequestType);
            Assert.Equal(mappedRequest.GetType(), request.GetType());
        }


        [Fact]
        public void ProcessRequest_DetectsAndDeserializesReqeustType_Delete()
        {
            var request = new DeleteRequest();
            byte[] requestBytes = SerializationUtils.Serialize(request);

            IRequestMapper processor = new RequestMapper();

            Request mappedRequest = processor.ProcessRequest(requestBytes, request.RequestType);
            Assert.Equal(mappedRequest.GetType(), request.GetType());
        }

    }
}
