using System;
using System.IO;
using Kronos.Shared.Network;

namespace Kronos.Client
{
    /// <summary>
    /// DataContract for KronosClient
    /// </summary>
    public interface IKronosClient : IDisposable
    {
        /// <summary>
        /// Write stream to Kronos node
        /// </summary>
        /// <param name="key">Stream identifier</param>
        /// <param name="stream">Stream to save in the Kronos</param>
        /// <param name="expiryDate">Stream Expiration date</param>
        /// <returns></returns>
        RequestStatusCode SaveInCache(string key, Stream stream, DateTime expiryDate);
    }
}
