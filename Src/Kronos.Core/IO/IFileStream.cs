using System;
using System.Collections.Generic;
using System.IO;

namespace Kronos.Core.IO
{
    internal interface IFileStream : IDisposable
    {
        long Position { get; }
        long Length { get; }
        
        void Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
        void Flush();
        void Seek(long offset, SeekOrigin seekOrigin);

        IEnumerable<string> EnumerateLines();
    }
}
