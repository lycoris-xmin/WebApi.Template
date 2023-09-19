using Lycoris.Api.Application.AppService.Authentication.Dtos;
using Lycoris.Api.Application.Cached.Dtos;
using Lycoris.Api.Application.Shared;

namespace Lycoris.Api.Application.AppService.Authentication
{
    public interface IAuthenticationAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<LoginValidateDto> LoginValidateAsync(string account, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remember"></param>
        /// <returns></returns>
        Task<LoginDto> LoginAsync(LoginValidateDto input, bool remember);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<string> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<LoginUserCacheDto?> GetLoginUserAsync(string token);
    }
}
