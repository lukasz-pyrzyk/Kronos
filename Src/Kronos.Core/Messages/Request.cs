using System.IO;
using ZeroFormatter;
using BufferedStream = Kronos.Core.Pooling.BufferedStream;

namespace Kronos.Core.Messages
{
    public class Request
    {
        public Auth Auth { get; set; }
        public RequestType Type { get; set; }
        public IRequest InternalRequest { get; set; }

        public void WriteTo(BufferedStream stream)
        {
            ZeroFormatterSerializer.Serialize(stream, Type);
            ZeroFormatterSerializer.Serialize(stream, Auth);
            ZeroFormatterSerializer.Serialize(stream, InternalRequest);
        }

        public static Request ParseFrom(byte[] data, int offset, int packageSize)
        {
            var stream = new MemoryStream(data, offset, packageSize);
            var request = new Request();
            request.Auth = ZeroFormatterSerializer.Deserialize<Auth>(stream);
            request.Type = ZeroFormatterSerializer.Deserialize<RequestType>(stream);
            request.InternalRequest = ZeroFormatterSerializer.Deserialize<IRequest>(stream);
            return request;
        }
    }

    public class Response
    {
        public string Exception { get; set; }

        public bool Success => string.IsNullOrEmpty(Exception);
        public IResponse InternalResponse { get; set; }

        public void WriteTo(BufferedStream stream)
        {
            ZeroFormatterSerializer.Serialize(stream, Exception);
            ZeroFormatterSerializer.Serialize(stream, InternalResponse);
        }

        public static Response ParseFrom(byte[] requestBytes, int offset, int packageSize)
        {
            var stream = new MemoryStream(requestBytes, offset, packageSize);
            var response = new Response();
            response.Exception = ZeroFormatterSerializer.Deserialize<string>(stream);
            response.InternalResponse = ZeroFormatterSerializer.Deserialize<IResponse>(stream);
            return response;
        }
    }
}
