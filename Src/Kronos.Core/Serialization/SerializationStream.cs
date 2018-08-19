using System;
using System.Runtime.InteropServices;
using System.Text;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;

namespace Kronos.Core.Serialization
{
    public struct SerializationStream
    {
        private readonly Memory<byte> _memory;
        private int _position;
        public int Length => _position;


        public SerializationStream(Memory<byte> memory)
        {
            _memory = memory;
            _position = 4;
        }

        public void Write(bool content)
        {
            var value = content ? 1 : 0;
            Write(value);
        }

        public void Write(string content)
        {
            if (content == null)
            {
                Write((byte)SerializationMeta.Null);
                return;
            }

            Write((byte)SerializationMeta.Notnull);
            Span<byte> bytes = stackalloc byte[content.Length];
            Encoding.UTF8.GetBytes(content, bytes);
            WriteWithPrefixLength(bytes);
        }

        public void Write(ReadOnlySpan<byte> content)
        {
            var destination = _memory.Span.Slice(_position, content.Length);
            content.CopyTo(destination);
            _position += content.Length;
        }

        public void WriteWithPrefixLength(ReadOnlySpan<byte> content)
        {
            Write(content.Length);
            Write(content);
        }

        public void Write(byte content)
        {
            _memory.Span[_position] = content;
            _position++;
        }

        public void Write(int value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            MemoryMarshal.Write(bytes, ref value);
            Write(bytes);
        }

        public void Write(long value)
        {
            Span<byte> bytes = stackalloc byte [sizeof(long)];
            MemoryMarshal.Write(bytes, ref value);
            Write(bytes);
        }

        public void Write(short value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(short)];
            MemoryMarshal.Write(bytes, ref value);
            Write(bytes);
        }

        public void Write(RequestType content)
        {
            Write((short)content);
        }

        public void Write(DateTimeOffset? content)
        {
            if (content.HasValue)
            {
                Write((byte)SerializationMeta.Notnull);
                Write(content.Value.Ticks);
            }
            else
            {
                Write((byte)SerializationMeta.Null);
            }
        }

        public void Flush()
        {
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            int length = _position - sizeof(int);
            MemoryMarshal.Write(bytes, ref length);
            for (int i = 0; i < sizeof(int); i++)
            {
                _memory.Span[i] = bytes[i];
            }
        }

        public ReadOnlyMemory<byte> MemoryWithLengthPrefix
        {
            get
            {
                if (_memory.Span[0] == 0) throw new KronosException("Stream empty or not flushed!");
                return _memory.Slice(0, _position);
            }
        }
    }
}
