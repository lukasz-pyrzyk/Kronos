using System;
using System.IO;
using Kronos.Shared.Network;

namespace Kronos.Client
{
    /// <summary>
    /// Official Kronos client
    /// <see cref="IKronosClient" />
    /// </summary>
    public class KronosClient : IKronosClient
    {
        /// <summary>
        /// <see cref="IKronosClient.SaveInCache"/>
        /// </summary>
        public RequestStatusCode SaveInCache(string key, Stream stream, DateTime expiryDate)
        {
            return RequestStatusCode.Ok;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
