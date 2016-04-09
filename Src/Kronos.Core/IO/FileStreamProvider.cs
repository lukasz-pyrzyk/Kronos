using System.Collections.Generic;
using System.IO;

namespace Kronos.Core.IO
{
    internal class FileStreamProvider : IFileStream
    {
        private readonly FileStream _stream;

        public FileStreamProvider(FileStream stream)
        {
            this._stream = stream;
        }

        public long Position => _stream.Position;
        public long Length => _stream.Length;
        public Stream Stream => _stream;

        public void Read(byte[] buffer, int offset, int count)
        {
            _stream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public void Flush()
        {
            _stream.Flush();
        }

        public void Seek(long offset, SeekOrigin seekOrigin)
        {
            _stream.Seek(offset, seekOrigin);
        }

        public IEnumerable<string> EnumerateLines()
        {
            StreamReader reader = new StreamReader(_stream);
            string line;
            do
            {
                line = reader.ReadLine();
                yield return line;

            } while (line != null);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public static IFileStream Open(string path)
        {
            FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            return new FileStreamProvider(stream);
        }
    }
}
