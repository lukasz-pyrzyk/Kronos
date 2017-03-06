using System;
using System.Collections.Generic;
using System.Linq;
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
            // Arrange
            var data = new Dictionary<Key, ByteString>
            {
                [new Key("first", DateTime.UtcNow.AddDays(-1))] = ByteString.CopyFromUtf8("first"),
                [new Key("second", DateTime.MaxValue)] = ByteString.CopyFromUtf8("second"),
                [new Key("third")] = ByteString.CopyFromUtf8("third"),
                [new Key("fourth", DateTime.UtcNow.AddDays(1))] = ByteString.CopyFromUtf8("fourth")
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


        private static PriorityQueue<Key> PrepareExpiringQueue(Dictionary<Key, ByteString> dic)
        {
            var queue = new PriorityQueue<Key>();

            foreach (var key in dic.Keys)
            {
                if (key.IsExpiring)
                {
                    queue.Add(key);
                }
            }

            return queue;
        }
    }
}
