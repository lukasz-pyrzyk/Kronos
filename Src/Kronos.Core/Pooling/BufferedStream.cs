using System;
using System.IO;
using Kronos.Core.Serialization;

namespace Kronos.Core.Pooling
{
    public class BufferedStream : Stream
    {
        public byte[] RawBytes => _pool;
        public bool IsClean => Position == InitialPosition;

        private const int InitialPosition = sizeof(int); // todo check if it gives 4th index

        private byte[] _pool;
        private long _position;

        public BufferedStream(int size)
        {
            _pool = new byte[InitialPosition + size];
            _position = InitialPosition;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _pool.Length;

        public override long Position
        {
            get => _position;
            set
            {
                if (value >= Length) throw new EndOfStreamException();
                _position = (int)value;
            }
        }

        public void Clean()
        {
            Position = InitialPosition;
        }

        public override void Flush()
        {
            WriteNewSize();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Array.Copy(_pool, (int)_position, buffer, offset, count);
            _position += count;

            return count;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public void SetPosition(int offset)
        {
            _position = InitialPosition + offset;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Array.Copy(buffer, offset, _pool, (int)_position, count);
            _position += count;
        }

        private void WriteNewSize()
        {
            // Package size without first bytes reserved for size
            int packageSize = (int)(Position - InitialPosition);

            // Write size to the reserved bytes without allocation
            NoAllocBitConverter.GetBytes(packageSize, _pool);
        }
    }
}