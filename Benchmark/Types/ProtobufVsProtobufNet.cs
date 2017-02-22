using System;
using BenchmarkDotNet.Attributes;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;

namespace Benchmark.Types
{
    public class ProtobufVsProtobufNet
    {
        [Params(1024, 8 * 1024, 8 * 1024 * 1024)]
        public int Size { get; set; }

        private InsertMessage googleMessage;
        private InsertRequest protobufNetMessage;

        [Setup]
        public void Setup()
        {
            byte[] message = new byte[Size];
            new Random().NextBytes(message);

            protobufNetMessage = new InsertRequest("lorem ipsum", message, DateTime.UtcNow);

            googleMessage = new InsertMessage
            {
                Key = "lorem ipsum",
                Data = ByteString.CopyFrom(message),
                Expiry = Timestamp.FromDateTime(DateTime.UtcNow)
            };
        }

        [Benchmark]
        public InsertMessage Google()
        {
            byte[] data = googleMessage.ToByteArray();

            var received = InsertMessage.Parser.ParseFrom(data);

            return received;
        }

        [Benchmark]
        public InsertRequest ProtobufNet()
        {
            byte[] data = SerializationUtils.Serialize(protobufNetMessage);

            var received = SerializationUtils.Deserialize<InsertRequest>(data);

            return received;
        }
    }
}
