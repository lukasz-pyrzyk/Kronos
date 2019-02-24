using System;
using System.Threading.Tasks;
using Kronos.Core.Messages;

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
        Task<InsertResponse> InsertAsync(string key, byte[] package, DateTimeOffset? expiryDate);

        /// <summary>
        /// Gets object from Kronos server
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<GetResponse> GetAsync(string key);

        /// <summary>
        /// Removes object from Kronos server
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<DeleteResponse> DeleteAsync(string key);

        /// <summary>
        /// Counts number of objects in the storage
        /// </summary>
        /// <returns>Number of objects in the storage</returns>
        Task<CountResponse> CountAsync();

        /// <summary>
        /// Checks if element exists in the storage
        /// </summary>
        /// <returns></returns>
        Task<ContainsResponse> ContainsAsync(string key);

        /// <summary>
        /// Clears the database
        /// </summary>
        /// <returns></returns>
        Task<ClearResponse> ClearAsync();

        /// <summary>
        /// Returns stats from the server
        /// </summary>
        /// <returns></returns>
        Task<StatsResponse> StatsAsync();
    }
}
