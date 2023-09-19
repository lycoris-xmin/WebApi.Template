using Lycoris.Api.Common;
using Lycoris.Api.Core.Cache;
using Lycoris.Api.Core.EntityFrameworkCore;
using Lycoris.Api.EntityFrameworkCore.Tables;
using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.CSRedisCore.Extensions;

namespace Lycoris.Api.Core.Repositories.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IRepository<Configuration, int> _repository;
        private readonly IMemoryCacheManager _memoryCache;

        public ConfigurationRepository(IRepository<Configuration, int> repository, IMemoryCacheManager memoryCache)
        {
            _repository = repository;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<Configuration?> GetDataAsync(string configId) => _repository.GetAsync(x => x.ConfigId == configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<string> GetConfigurationAsync(string configId)
        {
            var cache = await GetCacheAsync(configId);
            if (!cache.IsNullOrEmpty())
                return cache!;

            cache = await _repository.GetSelectAsync(x => x.ConfigId == configId, x => x.Value) ?? "";

            if (!cache.IsNullOrEmpty())
                await SetCacheAsync(configId, cache);

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<T?> GetConfigurationAsync<T>(string configId) where T : class
        {
            var cache = await GetCacheAsync(configId);
            if (!cache.IsNullOrEmpty())
                return cache!.ToObject<T>();

            cache = await _repository.GetSelectAsync(x => x.ConfigId == configId, x => x.Value) ?? "";

            if (!cache.IsNullOrEmpty())
                await SetCacheAsync(configId, cache);

            return cache?.ToObject<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task RemoveConfigurationCacheAsync(string configId)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.Key.RemoveAsync(GetCacheKey(configId));
            else
                _memoryCache.RemoveMemory(GetCacheKey(configId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        private async Task<string?> GetCacheAsync(string configId)
        {
            if (AppSettings.Redis.Use)
                return await RedisCache.String.GetAsync(GetCacheKey(configId));
            else
                return _memoryCache.GetMemory<string>(GetCacheKey(configId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task SetCacheAsync(string configId, string value)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetCacheKey(configId), value);
            else
                _memoryCache.CreateMemory(GetCacheKey(configId), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        private static string GetCacheKey(string configId) => $"Configuration:{configId}";
    }
}
