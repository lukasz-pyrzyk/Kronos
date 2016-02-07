using System;
using Kronos.Core.Model;
using Kronos.Core.StatusCodes;

namespace Kronos.Client
{
    /// <summary>
    /// DataContract for KronosClient
    /// </summary>
    public interface IKronosClient : IDisposable
    {
        /// <summary>
        /// Writes object to Kronos server
        /// </summary>
        /// <param name="key">Package identifier</param>
        /// <param name="package">Package to save in the Kronos</param>
        /// <param name="expiryDate">Package Expiration date</param>
        /// <returns>Request status code</returns>
        RequestStatusCode InsertToServer(string key, byte[] package, DateTime expiryDate);

        /// <summary>
        /// Writes object to Kronos server
        /// </summary>
        /// <param name="objectToCache">Object to serialize and save</param>
        /// <returns>Request status code</returns>
        RequestStatusCode InsertToServer(CachedObject objectToCache);

        /// <summary>
        /// Gets object from Kronos server
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] TryGetValue(string key);
    }
}
