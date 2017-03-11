using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Kronos.Core.Storage;
using Kronos.Core.Storage.Cleaning;
using Xunit;

namespace Kronos.Core.Tests.Storage.Cleaning
{
    public class CleanerTests
    {
        [Fact]
        public void Clear_CanDeleteObjectsFromStorage()
        {
            // Arrange
            var data = new Dictionary<Key, Element>
            {
                [new Key("first")] = new Element(ByteString.CopyFromUtf8("first"), DateTime.UtcNow.AddDays(-1)),
                [new Key("second")] = new Element(ByteString.CopyFromUtf8("second"), DateTime.UtcNow.AddDays(100)),
                [new Key("third")] = new Element(ByteString.CopyFromUtf8("third")),
                [new Key("fourth")] = new Element(ByteString.CopyFromUtf8("fourth"), DateTime.UtcNow.AddDays(1)),
            };

            int dataCountBefore = data.Count;

            var expiringKeys = PrepareExpiringQueue(data);
            int expiringKeysBefore = expiringKeys.Count;
            int shouldBeDeleted = expiringKeys.Count(x => x.IsExpired());

            Cleaner provider = new Cleaner();

            // Act
            provider.Clear(expiringKeys, data);

            // Assert
            int dataCountAfter = data.Count;
            int expiringKeysAfter = expiringKeys.Count;

            Assert.Equal(expiringKeysAfter, expiringKeysBefore - shouldBeDeleted);
            Assert.Equal(dataCountAfter, dataCountBefore - shouldBeDeleted);
        }


        private static PriorityQueue<ExpiringKey> PrepareExpiringQueue(Dictionary<Key, Element> dic)
        {
            var queue = new PriorityQueue<ExpiringKey>();

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
