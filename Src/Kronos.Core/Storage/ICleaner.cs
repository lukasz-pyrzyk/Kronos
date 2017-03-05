using System.Collections.Generic;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    public interface ICleaner
    {
        void Clear(Dictionary<Key, ByteString> nodes);
    }
}