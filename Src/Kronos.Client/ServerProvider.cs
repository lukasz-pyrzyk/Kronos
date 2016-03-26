using System;
using System.Collections.Generic;
using System.Linq;
using Kronos.Core.Configuration;

namespace Kronos.Client
{
    internal class ServerProvider
    {
        public int ServersCount => _clusterConfig.Servers.Length;
        public Dictionary<ushort, ServerConfig> Mappings;

        private readonly ClusterConfig _clusterConfig;

        public ServerProvider(ClusterConfig clusterConfig)
        {
            _clusterConfig = clusterConfig;
            InitializeMappings();
        }

        public ServerConfig SelectServer(int keyHashcode)
        {
            string stringHashCode = keyHashcode.ToString();
            ushort lastTwoDigits = Convert.ToUInt16(stringHashCode.Substring(stringHashCode.Length - 2, 2));
            return Mappings[lastTwoDigits];
        }

        private void InitializeMappings()
        {
            const ushort hashCodeRange = 100; // (0:99).
            Mappings = new Dictionary<ushort, ServerConfig>(hashCodeRange);

            ushort rangePerServer = (ushort)(hashCodeRange / ServersCount);
            ushort modulo = (ushort)(hashCodeRange % ServersCount);

            ushort position = 0;
            for (ushort i = 0; i < ServersCount; i++)
            {
                for (ushort j = 0; j < rangePerServer; j++)
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
