using System;
using System.Runtime.InteropServices;
using System.Text;
using Kronos.Core.Messages;

namespace Kronos.Core.Serialization
{
    public class DeserializationStream
    {
        private ReadOnlyMemory<byte> _data;
        private int _position = 0;

        public DeserializationStream(ReadOnlyMemory<byte> data)
        {
            _data = data;
        }

        public bool IsClean { get; set; }

        public int ReadInt()
        {
            int size = sizeof(int);
            int value = MemoryMarshal.Read<int>(_data.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public long ReadLong()
        {
            int size = sizeof(long);
            long value = MemoryMarshal.Read<long>(_data.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public short ReadShort()
        {
            int size = sizeof(short);
            var value = MemoryMarshal.Read<short>(_data.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public byte ReadByte()
        {
            return _data.Span[_position++];
        }

        public string ReadString()
        {
            var meta = (SerializationMeta) ReadByte();
            if (meta == SerializationMeta.Null)
            {
                return null;
            }

            var memory = ReadBytesWithLengthPrefix();
            var bytes = MemoryMarshal.AsBytes(memory);
            return Encoding.UTF8.GetString(bytes);
        }

        public ReadOnlySpan<byte> ReadBytesWithLengthPrefix()
        {
            int length = ReadInt();
            var bytes = _data.Span.Slice(_position, length);
            _position += length;
            return bytes;
        }

        public ReadOnlyMemory<byte> ReadMemory()
        {
            int length = ReadInt();
            var bytes = _data.Slice(_position, length);
            _position += length;
            return bytes;
        }

        public DateTimeOffset? ReadDateTimeOffset()
        {
            int size = sizeof(byte);
            var meta = MemoryMarshal.Read<SerializationMeta>(_data.Span.Slice(_position, size));
            _position += size;

            if (meta == SerializationMeta.Notnull)
            {
                var ticks = ReadLong();
                return new DateTimeOffset(new DateTime(ticks));
            }

            return null;
        }

        public bool ReadBoolean()
        {
            int size = sizeof(bool);
            var value = MemoryMarshal.Read<bool>(_data.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public RequestType ReadRequestType()
        {
            return (RequestType)ReadShort();
        }
    }
}
