using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Kronos.Server.Storage;
using Kronos.Server.Storage.Cleaning;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Storage.Cleaning
{
    public class CleanerTests
    {
        [Fact]
        public void Clear_CanDeleteObjectsFromStorage()
        {
            // Arrange
            var data = new Dictionary<Key, Element>
            {
                [new Key("first")] = new Element(ByteString.CopyFromUtf8("first"), DateTimeOffset.UtcNow.AddDays(-1)),
                [new Key("second")] = new Element(ByteString.CopyFromUtf8("second"), DateTimeOffset.UtcNow.AddDays(100)),
                [new Key("third")] = new Element(ByteString.CopyFromUtf8("third")),
                [new Key("fourth")] = new Element(ByteString.CopyFromUtf8("fourth"), DateTimeOffset.UtcNow.AddDays(1)),
            };

            int dataCountBefore = data.Count;

            var expiringKeys = PrepareExpiringQueue(data);
            int expiringKeysBefore = expiringKeys.Count;
            int shouldBeDeleted = expiringKeys.ToArray().Count(x => x.IsExpired());

            Cleaner provider = new Cleaner(Substitute.For<ILogger<Cleaner>>());

            // Act
            provider.Clear(expiringKeys, data);

            // Assert
            int dataCountAfter = data.Count;
            int expiringKeysAfter = expiringKeys.Count;

            Assert.Equal(expiringKeysAfter, expiringKeysBefore - shouldBeDeleted);
            Assert.Equal(dataCountAfter, dataCountBefore - shouldBeDeleted);
        }


        private static ConcurrentPriorityQueue<ExpiringKey> PrepareExpiringQueue(Dictionary<Key, Element> dic)
        {
            var queue = new ConcurrentPriorityQueue<ExpiringKey>();

            foreach (var pair in dic)
            {
                if (pair.Value.IsExpiring)
                {
                    queue.Add(new ExpiringKey(pair.Key, pair.Value.ExpiryDate.Value));
                }
            }

            return queue;
        }
    }
}
