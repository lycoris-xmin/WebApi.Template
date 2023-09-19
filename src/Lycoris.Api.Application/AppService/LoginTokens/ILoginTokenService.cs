namespace Lycoris.Api.Application.AppService.LoginTokens
{
    public interface ILoginTokenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        string GenereateToken(long userId, DateTime expiredTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        (long? userId, DateTime? expiredTime) AnalyzeToken(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        string GenereateRefreshToken(long userId, DateTime expiredTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        (long? userId, DateTime? expiredTime) AnalyzeRefreshToken(string refreshToken);
    }
}
