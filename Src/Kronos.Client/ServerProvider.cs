using System;
using System.Collections.Generic;
using Kronos.Core.Configuration;

namespace Kronos.Client
{
    internal class ServerProvider
    {
        public ServerConfig[] Servers => _clusterConfig.Servers;

        public Dictionary<byte, ServerConfig> Mappings;

        private readonly ClusterConfig _clusterConfig;

        public ServerProvider(ClusterConfig clusterConfig)
        {
            _clusterConfig = clusterConfig;
            InitializeMappings();
        }

        public ServerConfig SelectServer(int keyHashcode)
        {
            // get last two numbers, for example get 51 from 1989858951
            byte lastNumbers = (byte)Math.Abs(keyHashcode % 100);
            return Mappings[lastNumbers];
        }

        private void InitializeMappings()
        {
            const ushort hashCodeRange = 100; // numbers in range (0:99).
            Mappings = new Dictionary<byte, ServerConfig>(hashCodeRange);

            byte rangePerServer = (byte)(hashCodeRange / Servers.Length);
            byte modulo = (byte)(hashCodeRange % Servers.Length);

            byte position = 0;
            foreach (ServerConfig server in Servers)
            {
                // Assign range to the server
                for (int j = 0; j < rangePerServer; j++)
                {
                    Mappings[position] = server;
                    position++;
                }

                if (modulo != 0)
                {
                    Mappings[position] = server;
                    modulo--;
                    position++;
                }
            }
        }
    }
}
