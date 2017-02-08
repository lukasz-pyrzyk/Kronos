using System;

namespace ClusterBenchmark.Utils
{
    public static class Prepare
    {
        public static byte[] Bytes(int count)
        {
            var data = new byte[count];
            new System.Random().NextBytes(data);
            return data;
        }

        public static string Key()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
