﻿using System;
using System.IO;
using Kronos.Core.Exceptions;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct Request : ISerializable<Request>
    {
        public Auth Auth { get; set; }

        public RequestType Type { get; set; }

        public IRequest InternalRequest { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.Write(Type);
            Auth.Write(stream);
            InternalRequest.Write(stream);
        }

        public void Read(DeserializationStream stream)
        {
            Type = stream.ReadRequestType();
            var a = new Auth();
            a.Read(stream);
            Auth = a;
            switch (Type)
            {
               case RequestType.Insert:
                   InternalRequest = new InsertRequest();
                    break;
                case RequestType.Get:
                    InternalRequest = new GetRequest();
                    break;
                case RequestType.Delete:
                    InternalRequest = new DeleteRequest();
                    break;
                case RequestType.Count:
                    InternalRequest = new CountRequest();
                    break;
                case RequestType.Contains:
                    InternalRequest = new ContainsRequest();
                    break;
                case RequestType.Clear:
                    InternalRequest = new ClearRequest();
                    break;
                case RequestType.Stats:
                    InternalRequest = new StatsRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            InternalRequest.Read(stream);
        }
    }

    public class Response : ISerializable<object>
    {
        public string Exception { get; set; }

        public RequestType Type { get; set; }

        public bool Success => string.IsNullOrEmpty(Exception);

        public IResponse InternalResponse { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.Write(Type);
            stream.Write(Exception);
            InternalResponse?.Write(stream);
        }

        public void Read(DeserializationStream stream)
        {
            Type = stream.ReadRequestType();
            Exception = stream.ReadString();

            if (Exception != null) return;

            switch (Type)
            {
                case RequestType.Insert:
                    InternalResponse = new InsertResponse();
                    break;
                case RequestType.Get:
                    InternalResponse = new GetResponse();
                    break;
                case RequestType.Delete:
                    InternalResponse = new DeleteResponse();
                    break;
                case RequestType.Count:
                    InternalResponse = new CountResponse();
                    break;
                case RequestType.Contains:
                    InternalResponse = new ContainsResponse();
                    break;
                case RequestType.Clear:
                    InternalResponse = new ClearResponse();
                    break;
                case RequestType.Stats:
                    InternalResponse = new StatsResponse();
                    break;
                default:
                    throw new KronosException($"Unable to find request type for {InternalResponse?.GetType()}");
            }

            InternalResponse.Read(stream);
        }
    }
}
