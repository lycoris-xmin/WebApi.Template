using Lycoris.Api.Core.Logging;
using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;

namespace Lycoris.Api.Application.AppService.LoginTokens.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class LoginTokenService : ILoginTokenService
    {
        private const string KEY = "2B9E8A3F";
        private const string IV = "20220101";
        private readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public LoginTokenService(ILycorisLoggerFactory factory) => _logger = factory.CreateLogger<LoginTokenService>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public string GenereateToken(long userId, DateTime expiredTime)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));
            else if (expiredTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var value = $"{Guid.NewGuid():N}|{userId}|{expiredTime:yyyy-MM-dd HH:mm:ss}|{Guid.NewGuid():N}";

            return SecretHelper.DESEncrypt(SecretHelper.CommonEncrypt(value), KEY, IV);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public (long? userId, DateTime? expiredTime) AnalyzeToken(string token)
        {
            try
            {
                var value = SecretHelper.DESDecrypt(SecretHelper.CommonDecrypt(token), KEY, IV);
                var tmp = value?.Split('|') ?? Array.Empty<string>();
                if (tmp.Length != 4)
                    return (null, null);

                return (tmp[1].ToTryLong(), tmp[2].ToTryDateTime());
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
                return (null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public string GenereateRefreshToken(long userId, DateTime expiredTime)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));
            else if (expiredTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var value = $"{Guid.NewGuid():N}|{userId}|{expiredTime:yyyy-MM-dd HH:mm:ss}|{userId}|{Guid.NewGuid():N}";

            return SecretHelper.DESEncrypt(SecretHelper.CommonEncrypt(value), KEY, IV);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public (long? userId, DateTime? expiredTime) AnalyzeRefreshToken(string refreshToken)
        {
            try
            {
                var value = SecretHelper.DESDecrypt(SecretHelper.CommonDecrypt(refreshToken), KEY, IV);
                var tmp = value?.Split('|') ?? Array.Empty<string>();
                if (tmp.Length != 4)
                    return (null, null);

                return (tmp[1].ToTryLong(), tmp[2].ToTryDateTime());
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
                return (null, null);
            }
        }
    }
}
