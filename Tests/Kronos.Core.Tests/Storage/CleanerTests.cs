using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class CleanerTests
    {
        [Fact]
        public async Task Start_CanDeleteObjectsFromStorage()
        {
            var data = new Dictionary<Key, ByteString>
            {
                [new Key("one", DateTime.UtcNow)] = ByteString.CopyFromUtf8("first"),
                [new Key("two", DateTime.MaxValue)] = ByteString.CopyFromUtf8("second")
            };

            Cleaner provider = new Cleaner();
            provider.Start(data, CancellationToken.None);
            await Task.Delay(Cleaner.Timer + 100);

            Assert.Equal(data.Count, 1);
        }
    }
}
