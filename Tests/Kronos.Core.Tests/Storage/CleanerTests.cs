using System;
using System.Collections.Generic;
using Google.Protobuf;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class CleanerTests
    {
        [Fact]
        public void Clear_CanDeleteObjectsFromStorage()
        {
            var data = new Dictionary<Key, ByteString>
            {
                [new Key("one", DateTime.UtcNow)] = ByteString.CopyFromUtf8("first"),
                [new Key("two", DateTime.MaxValue)] = ByteString.CopyFromUtf8("second")
            };
            
            var expiringKeys = new PriorityQueue<Key>();

            Cleaner provider = new Cleaner();
            provider.Clear(expiringKeys, data);

            Assert.Equal(data.Count, 1);
        }
    }
}
