using System;
using System.Collections.Generic;
using System.Linq;
using Kronos.Core.Configuration;

namespace Kronos.Client
{
    internal class ServerProvider
    {
        public int ServersCount => _clusterConfig.Servers.Length;
        public Dictionary<int, ServerConfig> Mappings;

        private readonly ClusterConfig _clusterConfig;

        public ServerProvider(ClusterConfig clusterConfig)
        {
            _clusterConfig = clusterConfig;
            InitializeMappings();
        }

        public ServerConfig SelectServer(int keyHashcode)
        {
            string stringHashCode = keyHashcode.ToString();
            int lastTwoDigits = Convert.ToInt32(stringHashCode.Substring(stringHashCode.Length - 2, 2));
            return Mappings[lastTwoDigits];
        }

        private void InitializeMappings()
        {
            int hashCodeRange = 100; // maximum combination of two last digits from hashcode.

            int modulo = hashCodeRange % ServersCount;
            int rangePerServer = (int)(hashCodeRange / ServersCount);

            Mappings = new Dictionary<int, ServerConfig>(hashCodeRange);

            int position = 0;
            for (int i = 0; i < ServersCount; i++)
            {
                for (int j = 0; j < rangePerServer; j++)
                {
                    Mappings[position] = _clusterConfig.Servers.ElementAt(i);
                    position++;
                }

                if (modulo != 0)
                {
                    Mappings[position] = _clusterConfig.Servers.ElementAt(i);
                    modulo--;
                    position++;
                }
            }
        }
    }
}
