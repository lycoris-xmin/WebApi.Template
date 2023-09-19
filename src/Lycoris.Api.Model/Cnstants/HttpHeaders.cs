namespace Lycoris.Api.Model.Cnstants
{
    /// <summary>
    /// 头部信息Key常量
    /// </summary>
    public class HttpHeaders
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Origin = "Origin";

        /// <summary>
        /// 
        /// </summary>
        public const string Referer = "Referer";

        /// <summary>
        /// 请求域名
        /// </summary>
        public const string Host = "Host";

        /// <summary>
        /// Nginx真实来源请求头
        /// </summary>
        public const string NginxRemoteIp = "X-Real-IP";

        /// <summary>
        /// 
        /// </summary>
        public const string Forwarded = "X-Forwarded-For";

        /// <summary>
        /// 
        /// </summary>
        public const string UserAgent = "User-Agent";

        /// <summary>
        /// 
        /// </summary>
        public const string RequestIP = "RequestIP";

        /// <summary>
        /// 
        /// </summary>
        public const string RequestIPAddress = "RequestIPAddress";

        /// <summary>
        /// 登录令牌
        /// </summary>
        public const string Authentication = "X-Real-User";

        /// <summary>
        /// 
        /// </summary>
        public const string XSRFToken = "X-Lycoris-Token";
    }
}
