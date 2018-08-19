﻿using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct StatsRequest : IRequest
    {
        public static Request New(Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Stats,
                InternalRequest = new StatsRequest()
            };
        }

        public void Write(ref SerializationStream stream)
        {
        }

        public void Read(ref DeserializationStream stream)
        {
        }
    }

    public class StatsResponse : IResponse
    {
        public int MemoryUsed { get; set; }

        public long Elements { get; set; }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(MemoryUsed);
            stream.Write(Elements);
        }

        public void Read(ref DeserializationStream stream)
        {
            MemoryUsed = stream.ReadInt();
            Elements = stream.ReadInt();
        }
    }
}
