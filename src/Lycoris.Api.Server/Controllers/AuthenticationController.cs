using Lycoris.Api.Application.AppService.Authentication;
using Lycoris.Api.Application.Cached;
using Lycoris.Api.Model.Exceptions;
using Lycoris.Api.Model.Global.Output;
using Lycoris.Api.Server.Application.Swaggers;
using Lycoris.Api.Server.FilterAttributes;
using Lycoris.Api.Server.Models.Authentication;
using Lycoris.Api.Server.Shared;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Api.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationAppService _authentication;
        private readonly IAuthenticationCacheService _cache;

        /// <summary>
        /// 
        /// </summary>
        public AuthenticationController(IAuthenticationAppService authentication, IAuthenticationCacheService cache)
        {
            _authentication = authentication;
            _cache = cache;
        }

        /// <summary>
        /// 帐号密码验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login/Validate")]
        [ExcludeSwaggerHeader, Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginValidateViewModel>> LoginValidate([FromBody] LoginValidateInput input)
        {
            var dto = await _authentication.LoginValidateAsync(input.Account!, input.Password!);
            var oathCode = RandomHelper.GetRandomLetterStringLower(32);

            // 缓存记录
            await _cache.SetLoginOathCodeAsync(input.Account!, oathCode, dto);

            return Success(new LoginValidateViewModel(oathCode));
        }

        /// <summary>
        /// 授权码登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ExcludeSwaggerHeader, Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> Login([FromBody] LoginInput input)
        {
            var cache = await _cache.GetLoginOathCodeAsync(input.Account!) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, "oathcode is expired");
            if (cache.OathCode != input.OathCode)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var dto = await _authentication.LoginAsync(cache.Value!, input.Remember ?? false);

            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Logout()
        {
            await _authentication.LogoutAsync();
            return Success();
        }

        /// <summary>
        /// 访问令牌刷新
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Refresh/Token")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<RefreshTokenViewModel>> RefreshToken([FromBody] RefreshTokenInput input)
        {
            var dto = await _authentication.RefreshTokenAsync(input.RefreshToken!);
            return Success(new RefreshTokenViewModel(dto));
        }
    }
}
