using System;
using System.Buffers;
using System.IO;
using Kronos.Core.Serialization;

namespace Kronos.Core.Pooling
{
    public class BufferedStream : Stream
    {
        public byte[] RawBytes => _pool;
        public bool IsClean => Position == initialPosition; // TODO and length is initial

        private const int initialPosition = sizeof(int);

        private byte[] _pool;
        private int _length;
        private int _position;
        private readonly int _reserve = 4 * 1024;

        public BufferedStream()
        {
            _pool = ArrayPool<byte>.Shared.Rent(_reserve);

            ResetPositions();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set
            {
                if (value > Length) throw new EndOfStreamException();
                _position = (int)value;
            }
        }

        public void Clean()
        {
            ResetPositions();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position + count > _length)
            {
                throw new EndOfStreamException();
            }

            Copy(_pool, (int)Position, buffer, offset, count);

            Position += count;

            return count;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = initialPosition + offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = _length - offset;
                    break;
            }

            return Position;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Reallocate bytes if array is too small
            CheckSize(count);

            // Write data to the buffer
            Copy(buffer, offset, _pool, (int)Position, count);

            _length += count;
            Position += count;

            WriteNewSize();
        }

        private void CheckSize(int incomingBytes)
        {
            long expectedSize = Position + incomingBytes;
            if (expectedSize > _pool.Length)
            {
                // rent new, bigger array with expected size and some reserve
                byte[] newArray = Rent((int)expectedSize + _reserve);

                // copy bytes to new array
                Copy(_pool, 0, newArray, 0, _length);

                // return old bytes to the pool
                Return(_pool);

                _pool = newArray;
            }
        }

        private void WriteNewSize()
        {
            // Package size without first bytes reserved for size
            int packageSize = _length - initialPosition;

            // Write size to the reserved bytes without allocation
            NoAllocBitConverter.GetBytes(packageSize, _pool);
        }

        private void ResetPositions()
        {
            _length = initialPosition;
            Position = initialPosition;
        }

        protected override void Dispose(bool disposing)
        {
            Return(_pool);
            _pool = new byte[0];
        }

        private static void Copy(Array src, int srcOffset, Array dst, int dstOffset, int count)
        {
            Array.Copy(src, srcOffset, dst, dstOffset, count);
        }

        private static byte[] Rent(int count) => ArrayPool<byte>.Shared.Rent(count);

        private static void Return(byte[] bytes) => ArrayPool<byte>.Shared.Return(bytes);
    }
}