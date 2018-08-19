using System;
using Kronos.Core.Exceptions;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class Request : ISerializable<Request>
    {
        public Auth Auth { get; set; }

        public RequestType Type { get; set; }

        public IRequest InternalRequest { get; set; }

        public int CalculateSize()
        {
            int baseSize = 1024 * 500;
            if (Type == RequestType.Insert)
            {
                baseSize += ((InsertRequest)InternalRequest).Data.Length;
            }

            return baseSize;
        }


        public void Write(ref SerializationStream stream)
        {
            stream.Write(Type);
            Auth.Write(ref stream);
            InternalRequest.Write(ref stream);
        }

        public void Read(ref DeserializationStream stream)
        {
            Type = stream.ReadRequestType();
            Auth = new Auth();
            Auth.Read(ref stream);

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

            InternalRequest.Read(ref stream);
        }
    }

    public class Response : ISerializable<object>
    {
        public ReadOnlyMemory<byte> Exception { get; set; }

        public RequestType Type { get; set; }

        public bool Success => Exception.IsEmpty;

        public IResponse InternalResponse { get; set; }

        public int CalculateSize()
        {
            int baseSize = 1024 * 500;
            if (Type == RequestType.Get)
            {
                baseSize += ((GetResponse)InternalResponse).Data.Length;
            }

            return baseSize;
        }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Type);
            stream.WriteWithPrefixLength(Exception.Span);
            InternalResponse?.Write(ref stream);
        }

        public void Read(ref DeserializationStream stream)
        {
            Type = stream.ReadRequestType();

            Exception = stream.ReadMemoryWithLengthPrefix();

            if (!Exception.IsEmpty) return;

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

            InternalResponse.Read(ref stream);
        }
    }
}
