using System.IO;
using ZeroFormatter;
using BufferedStream = Kronos.Core.Pooling.BufferedStream;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class Request
    {
        [Index(0)]
        public virtual Auth Auth { get; set; }

        [Index(2)]
        public virtual IRequest InternalRequest { get; set; }

        public void WriteTo(BufferedStream stream)
        {
            ZeroFormatterSerializer.Serialize(stream, this);
        }

        public static Request ParseFrom(byte[] data, int offset, int packageSize)
        {
            var stream = new MemoryStream(data, offset, packageSize);
            return ZeroFormatterSerializer.Deserialize<Request>(stream);
        }
    }

    [ZeroFormattable]
    public class Response
    {
        [Index(0)]
        public virtual string Exception { get; set; }

        [IgnoreFormat]
        public virtual bool Success => string.IsNullOrEmpty(Exception);

        [Index(1)]
        public virtual IResponse InternalResponse { get; set; }

        public void WriteTo(BufferedStream stream)
        {
            ZeroFormatterSerializer.Serialize(stream, this);
        }

        public static Response ParseFrom(byte[] requestBytes, int offset, int packageSize)
        {
            var stream = new MemoryStream(requestBytes, offset, packageSize);
            return ZeroFormatterSerializer.Deserialize<Response>(stream);
        }
    }
}
