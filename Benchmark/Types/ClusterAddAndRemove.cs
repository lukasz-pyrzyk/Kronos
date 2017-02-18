﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Benchmark.Config;
using Benchmark.Utils;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Types
{
    public class ClusterAddAndRemove : ClusterBenchmark
    {
        [Params(Size.Mb, Size.Mb * 4)]
        public int PackageSize { get; set; }

        private byte[] _data;

        protected override void AdditionalSetup()
        {
            _data = Prepare.Bytes(PackageSize);

            Task kronos = KronosClient.ClearAsync();
            Task[] redisTasks = RedisServers.Select(x => x.FlushAllDatabasesAsync()).ToArray();
            Task.WaitAll(redisTasks);
            Task.WaitAll(kronos);
        }

        [Benchmark]
        public async Task Kronos()
        {
            string key = Prepare.Key();
            await KronosClient.InsertAsync(key, _data, DateTime.UtcNow.AddMinutes(5))
                .ConfigureAwait(false);

            await KronosClient.DeleteAsync(key)
                .ConfigureAwait(false);
        }

        [Benchmark]
        public async Task Redis()
        {
            string key = Prepare.Key();
            await RedisClient.SetAddAsync(key, _data)
                .ConfigureAwait(false);

            await RedisClient.KeyDeleteAsync(key)
                .ConfigureAwait(false);
        }
    }
}
