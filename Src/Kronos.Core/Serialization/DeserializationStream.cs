using System;
using System.Runtime.InteropServices;
using System.Text;
using Kronos.Core.Messages;

namespace Kronos.Core.Serialization
{
    public struct DeserializationStream
    {
        private ReadOnlyMemory<byte> _buffer;
        private int _position;

        public DeserializationStream(ReadOnlyMemory<byte> buffer)
        {
            _buffer = buffer;
            _position = 0;
        }

        public int ReadInt()
        {
            int size = sizeof(int);
            int value = MemoryMarshal.Read<int>(_buffer.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public long ReadLong()
        {
            int size = sizeof(long);
            long value = MemoryMarshal.Read<long>(_buffer.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public short ReadShort()
        {
            int size = sizeof(short);
            var value = MemoryMarshal.Read<short>(_buffer.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public byte ReadByte()
        {
            return _buffer.Span[_position++];
        }

        public string ReadString()
        {
            var meta = (SerializationMeta)ReadByte();
            if (meta == SerializationMeta.Null)
            {
                return null;
            }

            var memory = ReadBytesWithLengthPrefix();
            return memory.GetString();
        }

        public ReadOnlySpan<byte> ReadBytesWithLengthPrefix()
        {
            int length = ReadInt();
            var bytes = _buffer.Span.Slice(_position, length);
            _position += length;
            return bytes;
        }

        public ReadOnlyMemory<byte> ReadMemoryWithLengthPrefix()
        {
            int length = ReadInt();
            var bytes = _buffer.Slice(_position, length);
            _position += length;
            return bytes;
        }

        public ReadOnlyMemory<byte> ReadMemory()
        {
            int length = ReadInt();
            var bytes = _buffer.Slice(_position, length);
            _position += length;
            return bytes;
        }

        public DateTimeOffset? ReadDateTimeOffset()
        {
            int size = sizeof(byte);
            var meta = MemoryMarshal.Read<SerializationMeta>(_buffer.Span.Slice(_position, size));
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
            var value = MemoryMarshal.Read<bool>(_buffer.Span.Slice(_position, size));
            _position += size;
            return value;
        }

        public RequestType ReadRequestType()
        {
            return (RequestType)ReadShort();
        }
    }
}
