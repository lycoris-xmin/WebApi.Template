using Lycoris.Api.Application.AppService.Authentication.Dtos;
using Lycoris.Api.Application.AppService.LoginTokens;
using Lycoris.Api.Application.Cached;
using Lycoris.Api.Application.Cached.Dtos;
using Lycoris.Api.Application.Shared.Impl;
using Lycoris.Api.Core.EntityFrameworkCore;
using Lycoris.Api.EntityFrameworkCore.Common.Impl;
using Lycoris.Api.EntityFrameworkCore.Tables;
using Lycoris.Api.EntityFrameworkCore.Tables.Enums;
using Lycoris.Api.Model.Exceptions;
using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using System.Net;

namespace Lycoris.Api.Application.AppService.Authentication.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class AuthenticationAppService : ApplicationBaseService, IAuthenticationAppService
    {
        private readonly IRepository<User, long> _user;
        private readonly IRepository<LoginToken, long> _loginToken;
        private readonly Lazy<ILoginTokenService> _loginTokenService;
        private readonly Lazy<IAuthenticationCacheService> _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginToken"></param>
        /// <param name="loginTokenService"></param>
        /// <param name="cache"></param>
        public AuthenticationAppService(IRepository<User, long> user,
                                        IRepository<LoginToken, long> loginToken,
                                        Lazy<ILoginTokenService> loginTokenService,
                                        Lazy<IAuthenticationCacheService> cache)
        {
            _user = user;
            _loginToken = loginToken;
            _loginTokenService = loginTokenService;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<LoginValidateDto> LoginValidateAsync(string account, string password)
        {
            var data = await _user.GetAsync(x => x.Account == account) ?? throw new FriendlyException("帐号或密码错误", $"not find user with account:{account}");

            if (data.Password != SqlPasswrodConverter.Encrypt(password))
                throw new FriendlyException("帐号或密码错误", $"account:{account} password verification failed");
            else if (data.Status == UserStatusEnum.Defalut)
                throw new FriendlyException("帐号还未通过审核", $"account:{account} has not been approved");

            return data.ToMap<LoginValidateDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remember"></param>
        /// <returns></returns>
        public async Task<LoginDto> LoginAsync(LoginValidateDto input, bool remember)
        {
            var dto = input.ToMap<LoginDto>();

            var data = await _loginToken.GetAsync(input.Id!.Value) ?? new LoginToken();

            if (data.Id == input.Id!.Value)
            {
                // 数据库如果记录的缓存未过期，则删除就缓存
                if (data.TokenExpireTime > DateTime.Now)
                    await _cache.Value.SetLogoutStateAsync(data.Token);
            }

            // 生成访问令牌
            data.TokenExpireTime = DateTime.Now.AddMinutes(31);
            data.Token = _loginTokenService.Value.GenereateToken(input.Id.Value, data.TokenExpireTime);

            // 生成刷新令牌
            data.RefreshTokenExpireTime = DateTime.Now.AddDays(remember ? 15 : 1);
            data.RefreshToken = _loginTokenService.Value.GenereateRefreshToken(input.Id.Value, data.RefreshTokenExpireTime);

            // 插入数据库
            if (data.Id == 0)
            {
                data.Id = input.Id!.Value;
                await _loginToken.CreateAsync(data);
            }
            else
                await _loginToken.UpdateAsync(data);

            dto.Token = data.Token;
            dto.TokenExpireTime = data.TokenExpireTime;
            dto.RefreshToken = data.RefreshToken;

            // 设置登录令牌缓存
            await _cache.Value.SetLoginStateAsync(dto);

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            if (!this.LoginState || this.CurrentRequest.Token.IsNullOrEmpty())
                return;

            var (userId, expiredTime) = _loginTokenService.Value.AnalyzeToken(CurrentRequest.Token);

            if (!userId.HasValue || (expiredTime.HasValue && expiredTime.Value <= DateTime.Now))
                return;

            var data = await _loginToken.GetAsync(userId.Value);
            if (data == null)
                return;

            if (data.Token != CurrentRequest.Token)
                throw new FriendlyException("退出登录失败", $"token not match,database:{data.Token},request:{CurrentRequest.Token}");

            await _cache.Value.SetLogoutStateAsync(data.Token);

            data.TokenExpireTime = DateTime.Now.AddSeconds(-1);
            data.RefreshTokenExpireTime = DateTime.Now.AddSeconds(-1);
            await _loginToken.UpdateFieIdsAsync(data, x => x.TokenExpireTime, x => x.RefreshTokenExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var (userId, expiredTime) = _loginTokenService.Value.AnalyzeRefreshToken(refreshToken);
            if (!userId.HasValue || !expiredTime.HasValue)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "token parsing failed, does not comply with system generation logic");

            var data = await _loginToken.GetAsync(userId.Value) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"not find token data with user id:{userId}");

            if (data.RefreshToken != refreshToken || data.RefreshTokenExpireTime <= DateTime.Now)
                throw new HttpStatusException(HttpStatusCode.Unauthorized, $"refresh token not match,database:{data.RefreshToken},request:{refreshToken}");

            var cache = await _cache.Value.GetLoginStateAsync(data.Token);
            if (cache == null)
            {
                cache = await _user.GetSelectAsync(userId!.Value, x => new LoginUserCacheDto()
                {
                    Id = x.Id,
                    NickName = x.NickName,
                    Avatar = x.Avatar,
                    RoleId = x.RoleId
                });
            }
            else
                await _cache.Value.SetLogoutStateAsync(data.Token);

            // 生成访问令牌
            data.TokenExpireTime = DateTime.Now.AddMinutes(31);
            data.Token = _loginTokenService.Value.GenereateToken(userId.Value, data.TokenExpireTime);

            // 刷新刷新令牌的过期时间
            if (data.RefreshTokenExpireTime.AddHours(-12) < DateTime.Now)
                data.RefreshTokenExpireTime = DateTime.Now.AddDays(1);

            await _loginToken.UpdateFieIdsAsync(data, x => x.Token, x => x.TokenExpireTime, x => x.RefreshTokenExpireTime);

            cache!.TokenExpireTime = data.TokenExpireTime;

            // 写入缓存
            await _cache.Value.SetLoginStateAsync(data.Token, cache);

            return data.Token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<LoginUserCacheDto?> GetLoginUserAsync(string token)
        {

            var cache = await _cache.Value.GetLoginStateAsync(token);
            if (cache != null && cache.TokenExpireTime > DateTime.Now)
                return cache;

            var (userId, expitedTime) = _loginTokenService.Value.AnalyzeToken(token);
            if (userId == null || userId.Value == 0)
                return null;
            else if (expitedTime == null || expitedTime <= DateTime.Now)
                return null;

            var user = await _user.GetAsync(userId!.Value);
            if (user == null)
            {

                return null;
            }


            var data = await _loginToken.GetAsync(userId!.Value);
            if (data == null)
            {

                return null;
            }

            if (data.Token != token)
            {

                return null;
            }


            cache = new LoginUserCacheDto()
            {
                Id = user.Id,
                NickName = user.NickName,
                Avatar = user.Avatar,
                RoleId = user.RoleId,
                TokenExpireTime = data.TokenExpireTime
            };

            await _cache.Value.SetLoginStateAsync(token, cache);

            return cache;
        }
    }
}
