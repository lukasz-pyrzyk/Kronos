using System;
using System.Collections.Generic;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
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

            IRequestMapper processor = new RequestMapper();

            Exception ex =
                Assert.Throws<InvalidOperationException>(
                    () => processor.ProcessRequest(requestBytes, type));

            Assert.Equal(ex.Message, $"Cannot find processor for type {type}");
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ProcessRequest_DetectsAndDeserializesReqeustType(Request request)
        {
            byte[] requestBytes = SerializationUtils.Serialize(request);

            IRequestMapper processor = new RequestMapper();

            Request mappedRequest = processor.ProcessRequest(requestBytes, request.RequestType);
            Assert.Equal(mappedRequest.GetType(), request.GetType());
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new Request[] { new InsertRequest() };
            yield return new Request[] { new GetRequest(), };
            yield return new Request[] { new DeleteRequest() };
            yield return new Request[] { new CountRequest() };
            yield return new Request[] { new ContainsRequest() };
        }
    }
}
