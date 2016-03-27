using System;
using System.Text;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using ProtoBuf;
using Xunit;

namespace Kronos.Core.Tests.Serialization
{
    public class SerializationutilsTests
    {
        [Fact]
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("key"), DateTime.Now);

            byte[] array = SerializationUtils.Serialize(request);
            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(array);

            Assert.Equal(requestFromBytes.RequestType, request.RequestType);
            Assert.Equal(requestFromBytes.Key, requestFromBytes.Key);
            Assert.Equal(requestFromBytes.ExpiryDate, requestFromBytes.ExpiryDate);
            Assert.Equal(requestFromBytes.Object, requestFromBytes.Object);
        }

        [Fact]
        public void GetLengthOfPackage_ReturnsCorrectNumber()
        {
            string word = "lorem ipsum";
            byte[] buffer = SerializationUtils.Serialize(word);

            int expectedSize;
            Serializer.TryReadLengthPrefix(buffer, 0, buffer.Length, PrefixStyle.Base128, out expectedSize);

            int size = SerializationUtils.GetLengthOfPackage(buffer);

            Assert.Equal(size, expectedSize);
        }
    }
}
