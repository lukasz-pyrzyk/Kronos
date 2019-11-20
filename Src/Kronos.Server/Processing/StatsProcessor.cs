using System;
using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class StatsProcessor : CommandProcessor<StatsRequest, StatsResponse>
    {
        public override StatsResponse Reply(StatsRequest request, IStorage storage)
        {
            long usedBytes = 0;
            long count = 0;
            foreach (var element in storage)
            {
                usedBytes += element.Data.Length;
                count++;
            }
            return new StatsResponse
            {
                MemoryUsed = Mb(usedBytes),
                Elements = count,
            };
        }

        private static int Mb(double number)
        {
            return (int)Math.Floor(number / (1024 * 1024));
        }
    }
}
