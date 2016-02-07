using System;
using System.IO;
using Kronos.Core.Requests;
using ProtoBuf;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestTypeTests
    {
        [Theory]
        [InlineData(RequestType.InsertRequest)]
        [InlineData(RequestType.GetRequest)]
        public void CanSerializeAndDeserialize(RequestType type)
        {
            byte[] package;
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, type);
                package = ms.ToArray();
            }

            RequestType typeFromBytes;

            using (MemoryStream ms = new MemoryStream(package))
            {
                typeFromBytes = Serializer.Deserialize<RequestType>(ms);
            }

            Assert.Equal(type, typeFromBytes);
        }
    }
}
