using Lycoris.Api.Application.AppService.Authentication.Dtos;
using Lycoris.Api.Application.Cached.Dtos;
using Lycoris.Api.Common;
using Lycoris.Api.Core.Cache;
using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.CSRedisCore.Extensions;

namespace Lycoris.Api.Application.Cached.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class AuthenticationCacheService : IAuthenticationCacheService
    {
        private readonly Lazy<IMemoryCacheManager> _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        public AuthenticationCacheService(Lazy<IMemoryCacheManager> memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #region 授权码
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<LoginOathCodeDto?> GetLoginOathCodeAsync(string account)
        {
            if (AppSettings.Redis.Use)
                return await RedisCache.String.GetAsync<LoginOathCodeDto>(GetLoginOathCodeKey(account));
            else
                return _memoryCache.Value.GetMemory<LoginOathCodeDto>(GetLoginOathCodeKey(account));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="oathCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetLoginOathCodeAsync(string account, string oathCode, LoginValidateDto value)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetLoginOathCodeKey(account), new LoginOathCodeDto(oathCode, value), TimeSpan.FromSeconds(60));
            else
                _memoryCache.Value.CreateMemory(GetLoginOathCodeKey(account), new LoginOathCodeDto(oathCode, value), DateTime.Now.AddSeconds(60));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task RemoveLoginOathCodeAsync(string account)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.Key.RemoveAsync(GetLoginOathCodeKey(account));
            else
                _memoryCache.Value.RemoveMemory(GetLoginOathCodeKey(account));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private static string GetLoginOathCodeKey(string account) => $"OathCode:Login:{account}";
        #endregion

        #region 访问令牌
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<LoginUserCacheDto?> GetLoginStateAsync(string token)
        {
            if (AppSettings.Redis.Use)
                return await RedisCache.String.GetAsync<LoginUserCacheDto>(GetTokenKey(token));
            else
                return _memoryCache.Value.GetMemory<LoginUserCacheDto>(GetTokenKey(token));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SetLoginStateAsync(LoginDto input)
        {
            var value = input.ToMap<LoginUserCacheDto>();

            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetTokenKey(input.Token!), input, value.TokenExpireTime - DateTime.Now);
            else
                _memoryCache.Value.CreateMemory(GetTokenKey(input.Token!), input, value.TokenExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetLoginStateAsync(string token, LoginUserCacheDto value)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetTokenKey(token), value, value.TokenExpireTime - DateTime.Now);
            else
                _memoryCache.Value.CreateMemory(GetTokenKey(token), value, value.TokenExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task SetLogoutStateAsync(string token)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.Key.RemoveAsync(GetTokenKey(token));
            else
                _memoryCache.Value.RemoveMemory(GetTokenKey(token));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public async Task UpdateLoginStateAsync(string token, Action<LoginUserCacheDto> configure)
        {
            var key = GetTokenKey(token);

            var value = AppSettings.Redis.Use
                        ? await RedisCache.String.GetAsync<LoginUserCacheDto>(key)
                        : _memoryCache.Value.GetMemory<LoginUserCacheDto>(key);

            if (value == null || value.TokenExpireTime < DateTime.Now)
                return;

            configure(value);

            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(key, value, value.TokenExpireTime - DateTime.Now);
            else
                _memoryCache.Value.CreateMemory(key, value, value.TokenExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string GetTokenKey(string token) => $"Authentication:{token}";
        #endregion
    }
}
