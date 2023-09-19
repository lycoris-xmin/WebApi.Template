using Lycoris.Api.Application.AppService.Authentication.Dtos;
using Lycoris.Api.Application.Cached.Dtos;

namespace Lycoris.Api.Application.Cached
{
    public interface IAuthenticationCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="oathCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetLoginOathCodeAsync(string account, string oathCode, LoginValidateDto value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<LoginOathCodeDto?> GetLoginOathCodeAsync(string account);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task RemoveLoginOathCodeAsync(string account);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<LoginUserCacheDto?> GetLoginStateAsync(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SetLoginStateAsync(LoginDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetLoginStateAsync(string token, LoginUserCacheDto value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetLogoutStateAsync(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task UpdateLoginStateAsync(string token, Action<LoginUserCacheDto> configure);
    }
}
