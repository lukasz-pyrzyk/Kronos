using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class StorageExpiryProviderTests
    {
        [Fact]
        public void Start_CanDeleteObjectsFromStorage()
        {
            var data = new ConcurrentDictionary<NodeMetatada, byte[]>
            {
                [new NodeMetatada("one", DateTime.UtcNow)] = new byte[0],
                [new NodeMetatada("two", DateTime.MaxValue)] = new byte[0]
            };

            StorageExpiryProvider provider = new StorageExpiryProvider();
            provider.Start(data, CancellationToken.None);
            Thread.Sleep(StorageExpiryProvider.Timer + 100);

            Assert.Equal(data.Count, 1);
        }
    }
}
