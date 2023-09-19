using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Lycoris.Api.Core.Cache.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class MemoryCacheManager : IMemoryCacheManager
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IMemoryCache memoryCache;

        private readonly object lockobj = new();

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool CreateMemory<T>(string key, T data)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (memoryCache.TryGetValue(key, out _))
                memoryCache.Remove(key);

            memoryCache.Set(key, data);

            return memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <param name="expiration">过期时间(单位：秒)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool CreateMemory<T>(string key, T data, DateTime expiration)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (memoryCache.TryGetValue(key, out _))
                memoryCache.Remove(key);

            memoryCache.Set(key, data, expiration);

            return memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <param name="expiration">过期时间(单位：秒)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool CreateMemory<T>(string key, T data, TimeSpan expiration)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            memoryCache.Set(key, data, expiration);

            return memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="data">缓存Value</param>
        /// <param name="cacheOption">缓存配置项</param>
        /// <returns></returns>
        public bool CreateMemory<T>(string key, T data, MemoryCacheEntryOptions cacheOption)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            cacheOption.Size ??= 1;

            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (memoryCache.TryGetValue(key, out _))
                memoryCache.Remove(key);

            memoryCache.Set(key, data, cacheOption);

            return memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetMemory<T>(string key)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            return memoryCache.Get<T>(key);
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T? TryGetMemory<T>(string key)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            if (!memoryCache.TryGetValue(key, out object? keycache))
                return default;

            try
            {
                return (T?)keycache;
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 读取并设置缓存(如果缓存不存在)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T? GetOrCreateMemory<T>(string key, T data)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            return memoryCache.GetOrCreate(key, r => data);
        }

        /// <summary>
        /// 读取并设置缓存(如果缓存不存在)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T?> GetOrCreateMemoryAsync<T>(string key, Task<T> data)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            return await memoryCache.GetOrCreateAsync(key, r => data);
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool ExistsMemory(string key)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key));

            return memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveMemory(string key)
        {
            if (memoryCache.TryGetValue(key, out _))
            {
                memoryCache.Remove(key);
                return memoryCache.TryGetValue(key, out _);
            }
            else
                return true;
        }

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool EnqueueMemory<T>(string key, T value)
        {
            lock (lockobj)
            {
                var queue = GetMemory<Queue<T>>(key);
                queue ??= new Queue<T>();
                queue.Enqueue(value);
                return CreateMemory(key, queue, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
            }
        }

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? DequeueMemory<T>(string key)
        {
            lock (lockobj)
            {
                var queue = GetMemory<Queue<T>>(key);
                if (queue == null || queue.Count == 0)
                    return default;

                var res = queue.Dequeue();
                return CreateMemory(key, queue, TimeSpan.FromHours(6)) ? res : default;
            }
        }
    }
}
