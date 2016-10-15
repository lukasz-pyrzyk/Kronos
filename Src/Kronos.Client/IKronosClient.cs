using System;
using System.Threading.Tasks;

namespace Kronos.Client
{
    /// <summary>
    /// DataContract for KronosClient
    /// </summary>
    public interface IKronosClient
    {
        /// <summary>
        /// Writes object to Kronos server
        /// </summary>
        /// <param name="key">Package identifier</param>
        /// <param name="package">Package to save in the Kronos</param>
        /// <param name="expiryDate">Package Expiration date</param>
        Task InsertAsync(string key, byte[] package, DateTime expiryDate);

        /// <summary>
        /// Gets object from Kronos server
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<byte[]> GetAsync(string key);

        /// <summary>
        /// Removes object from Kronos server
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteAsync(string key);
    }
}
