using System;
using Kronos.Shared.Network.Codes;

namespace Kronos.Client.Core
{
    /// <summary>
    /// DataContract for KronosClient
    /// </summary>
    public interface IKronosClient : IDisposable
    {
        /// <summary>
        /// Write stream to Kronos server
        /// </summary>
        /// <param name="key">Package identifier</param>
        /// <param name="stream">Package to save in the Kronos</param>
        /// <param name="expiryDate">Package Expiration date</param>
        /// <returns>Request status code</returns>
        RequestStatusCode InsertToServer(string key, byte[] package, DateTime expiryDate);
    }
}
