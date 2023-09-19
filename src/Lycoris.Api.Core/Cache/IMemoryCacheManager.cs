using Microsoft.Extensions.Caching.Memory;

namespace Lycoris.Api.Core.Cache
{
    public interface IMemoryCacheManager
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <returns></returns>
        bool CreateMemory<T>(string key, T data);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <param name="expiration">过期时间(单位：秒)</param>
        /// <returns></returns>
        bool CreateMemory<T>(string key, T data, DateTime expiration);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <param name="expiration">过期时间(单位：秒)</param>
        /// <returns></returns>
        bool CreateMemory<T>(string key, T data, TimeSpan expiration);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <param name="cacheOption">缓存配置项</param>
        /// <returns></returns>
        bool CreateMemory<T>(string key, T data, MemoryCacheEntryOptions cacheOption);

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T? GetMemory<T>(string key);

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        T? TryGetMemory<T>(string key);

        /// <summary>
        /// 读取并设置缓存(如果缓存不存在)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        T? GetOrCreateMemory<T>(string key, T data);

        /// <summary>
        /// 读取并设置缓存(如果缓存不存在)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<T?> GetOrCreateMemoryAsync<T>(string key, Task<T> data);

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool ExistsMemory(string key);

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveMemory(string key);

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool EnqueueMemory<T>(string key, T value);

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T? DequeueMemory<T>(string key);
    }
}
