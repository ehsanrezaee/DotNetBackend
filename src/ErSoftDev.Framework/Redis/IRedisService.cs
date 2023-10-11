using StackExchange.Redis;

namespace ErSoftDev.Framework.Redis
{
    public interface IRedisService
    {
        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        Task<bool> AddOrUpdateAsync(string key, string value,
            ExpiryTime expiry = ExpiryTime.TenMinute, CommandFlags flags = CommandFlags.None);


        /// <summary>
        /// Set key to hold the string value for exactly second time. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expireTimeSecond">The exactly second time</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        Task<bool> AddOrUpdateAsync(string key, string value,
            long expireTimeSecond = 60L, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">Generic class that convert to json</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        Task<bool> AddOrUpdateAsync<T>(string key, T value, ExpiryTime expiry = ExpiryTime.TenMinute,
            CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">Generic class that convert to json</param>
        /// <param name="expireTimeSecond">The exactly second time</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        Task<bool> AddOrUpdateAsync<T>(string key, T value, long expireTimeSecond = 60,
            CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// Get the T class value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the key was removed.</returns>
        Task<bool> DeleteAsync(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes all keys that matched with pattern.
        /// </summary>
        /// <param name="key">The key to delete. pattern is key*</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the key was removed.</returns>
        Task<bool> DeleteWithLikeAsync(string key, CommandFlags flags = CommandFlags.None);
    }
}
