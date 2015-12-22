using System;
using System.IO;
using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;

namespace Kronos.Client
{
    /// <summary>
    /// DataContract for KronosClient
    /// </summary>
    public interface IKronosClient : IDisposable
    {
        /// <summary>
        /// Write stream to Kronos server
        /// </summary>
        /// <param name="key">Stream identifier</param>
        /// <param name="stream">Stream to save in the Kronos</param>
        /// <param name="expiryDate">Stream Expiration date</param>
        /// <returns>Request status code</returns>
        RequestStatusCode InsertToServer(string key, Stream stream, DateTime expiryDate);
    }
}
