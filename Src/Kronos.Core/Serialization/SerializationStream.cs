using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;

namespace Kronos.Core.Serialization
{
    public class SerializationStream : IDisposable
    {
        private readonly byte[] _bytes;
        private int _position = 4;

        public SerializationStream(int neededSize)
        {
            _bytes = ArrayPool<byte>.Shared.Rent(neededSize);
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
            var destination = new Span<byte>(_bytes, _position, content.Length);
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
            _bytes[_position] = content;
            _position++;
        }

        public void Write(int content)
        {
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            NoAllocBitConverter.GetBytes(content, bytes);
            Write(bytes);
        }

        public void Write(long content)
        {
            Span<byte> bytes = stackalloc byte [sizeof(long)];
            NoAllocBitConverter.GetBytes(content, bytes);
            Write(bytes);
        }

        public void Write(short content)
        {
            var bytes = BitConverter.GetBytes(content);
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
            var size = BitConverter.GetBytes(_position - 4);
            for (int i = 0; i < sizeof(int); i++)
            {
                _bytes[i] = size[i];
            }
        }

        public bool IsClean => _position == 0;

        public ReadOnlyMemory<byte> Memory => MemoryWithLengthPrefix.Slice(4, _position - 4);
        public ReadOnlyMemory<byte> MemoryWithLengthPrefix
        {
            get
            {
                if (_bytes[0] == 0) throw new KronosException("Stream ampty or not flushed!");

                return new ReadOnlyMemory<byte>(_bytes, 0, _position);
            }
        }

        public int Length => _position;

        public void Dispose()
        {
            Array.Clear(_bytes, 0, _position);
            ArrayPool<byte>.Shared.Return(_bytes);
        }
    }
}
