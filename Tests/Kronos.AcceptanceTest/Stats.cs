﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Stats : Base
    {
        [Fact]
        public override async Task RunAsync()
        {
            await RunInternalAsync();
        }

        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Arrange
            var elements = 5;
            var mbperElement = 2;
            var data = new byte[mbperElement * 1024 * 1024];
            for (int i = 0; i < elements; i++)
            {
                await client.InsertAsync(Guid.NewGuid().ToString(), data, null);
            }

            // Act
            var stats = await client.StatsAsync();

            // Assert
            var serverStats = stats.Single();
            serverStats.Should().NotBeNull();
            serverStats.Elements.Should().Be(elements);
            serverStats.MemoryUsed.Should().Be(elements * mbperElement);
        }
    }
}
