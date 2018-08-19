﻿using System.Net;
using System.Runtime.Serialization;
using Kronos.Core.Messages;
using Kronos.Core.Networking;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class ServerConfig
    {
        private IPEndPoint _endPoint;
        private Auth _authorization;

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public AuthConfig Credentials { get; set; }

        public Auth Authorization => _authorization ?? (_authorization = PrepareAuth());

        private Auth PrepareAuth()
        {
            if (Credentials != null)
            {
                return Auth.FromCfg(Credentials);
            }
            return Auth.Default();
        }

        public IPEndPoint EndPoint => _endPoint ?? (_endPoint = CreateIpEndPoint());

        private IPEndPoint CreateIpEndPoint()
        {
            IPAddress address;
            if (!string.IsNullOrEmpty(Domain))
            {
                address = EndpointUtils.GetIpAsync(Domain).Result;
                Ip = address.ToString();
            }
            else
            {
                address = IPAddress.Parse(Ip);
            }

            return new IPEndPoint(address, Port);
        }

        public override string ToString()
        {
            return EndPoint.ToString();
        }
    }
}
