﻿using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct ContainsRequest : IRequest
    {
        public string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Contains,
                InternalRequest = new ContainsRequest { Key = key }
            };
        }

        public void Write(SerializationStream stream)
        {
            stream.Write(Key);
        }

        public void Read(DeserializationStream stream)
        {
            Key = stream.ReadString();
        }
    }

    public class ContainsResponse : IResponse
    {
        public bool Contains { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.Write(Contains);
        }

        public void Read(DeserializationStream stream)
        {
            Contains = stream.ReadBoolean();
        }
    }
}