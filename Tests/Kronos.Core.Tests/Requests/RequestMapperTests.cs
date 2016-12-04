﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestMapperTests
    {
        [Fact]
        public async Task ProcessRequest_ThrowsWhenProcessorForRequestTypeIsNotDefined()
        {
            byte[] requestBytes = new byte[0];
            RequestType type = RequestType.Unknown;
            IStorage storage = Substitute.For<IStorage>();

            IRequestProcessor processor = new RequestProcessor(storage);

            var ex = await
                Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await processor.HandleAsync(type, requestBytes, requestBytes.Length, new Socket(SocketType.Stream, ProtocolType.IP)));

            Assert.Equal(ex.Message, $"Cannot find processor for type {type}");
        }

        [Theory(Skip = "Fix it")]
        [MemberData(nameof(TestData))]
        public async Task ProcessRequest_DetectsAndDeserializesReqeustType(IRequest request, object specificProcessor)
        {
            byte[] requestBytes = SerializationUtils.Serialize(request);
            IStorage storage = Substitute.For<IStorage>();
            Socket socket = Substitute.For<Socket>();

            IRequestProcessor processor = new RequestProcessor(storage);

            await processor.HandleAsync(request.Type, requestBytes, requestBytes.Length, socket);
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { new InsertRequest(), new InsertProcessor() };
            yield return new object[] { new GetRequest(), new GetProcessor() };
            yield return new object[] { new DeleteRequest(), new DeleteProcessor() };
            yield return new object[] { new CountRequest(), new CountProcessor() };
            yield return new object[] { new ContainsRequest(), new ContainsProcessor() };
        }
    }
}
