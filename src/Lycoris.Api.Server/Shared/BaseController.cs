using Lycoris.Api.Common.Extensions;
using Lycoris.Api.Core.Logging;
using Lycoris.Api.Model.Cnstants;
using Lycoris.Api.Model.Contexts;
using Lycoris.Api.Model.Global.Output;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Api.Server.Shared
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class BaseController : ControllerBase
    {
        private RequestContext? _requestContext = null;

        /// <summary>
        /// 请求上下文
        /// </summary>
        protected virtual RequestContext CurrentRequest
        {
            get
            {
                _requestContext ??= HttpContext.GetService<RequestContext>();
                return _requestContext;
            }
        }

        private ILycorisLogger? _logger = null;

        /// <summary>
        /// 
        /// </summary>
        protected virtual ILycorisLogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    var factory = HttpContext.GetService<ILycorisLoggerFactory>();
                    _logger = factory.CreateLogger(GetType());
                }

                return _logger;
            }
        }

        /// <summary>
        ///  获取cookie的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string? GetCookie(string key)
        {
            if (!HttpContext.Request.Cookies.TryGetValue(key, out string? value))
                value = string.Empty;

            return value;
        }

        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresTime">过期时间(分钟)</param>
        /// <param name="path">指定路径,默认根目录</param>
        /// <param name="domain">指定域名</param>
        protected virtual void SetCookie(string key, string value, double expiresTime = 300, string path = "/", string domain = "")
        {
            var opt = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expiresTime),
                Path = path,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true
            };

            if (!domain.IsNullOrEmpty())
                opt.Domain = domain;

            HttpContext.Response.Cookies.Append(key, value, opt);
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="key"></param>
        protected virtual void RemoveCookie(string key)
        {
            var opt = new CookieOptions
            {
                SameSite = SameSiteMode.None,
                Secure = true
            };

            HttpContext.Response.Cookies.Delete(key, opt);
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <returns></returns>
        protected virtual BaseOutput Success()
        {
            var res = new BaseOutput
            {
                ResCode = ResCodeEnum.Success,
                ResMsg = "",
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            return res;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual DataOutput<T> Success<T>(T? data) where T : class, new()
        {
            var obj = new DataOutput<T>
            {
                ResCode = ResCodeEnum.Success,
                ResMsg = "",
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId),
                Data = data
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, obj.ToJson());

            return obj;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual ListOutput<T> Success<T>(List<T>? list) where T : class, new()
        {
            var obj = new ListOutput<T>
            {
                ResCode = ResCodeEnum.Success,
                ResMsg = "",
                Data = new ListViewModel<T>(list),
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, obj.ToJson());

            return obj;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual PageOutput<T> Success<T>(int count, List<T>? list) where T : class, new()
        {
            var obj = new PageOutput<T>
            {
                ResCode = ResCodeEnum.Success,
                ResMsg = "",
                Data = new PageViewModel<T>(count, list),
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, obj.ToJson());

            return obj;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="summary"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual PageOutput<T> Success<T>(int count, T? summary, List<T>? list) where T : class, new()
        {
            var obj = new PageOutput<T>
            {
                ResCode = ResCodeEnum.Success,
                ResMsg = "",
                Data = new PageViewModel<T>(count, summary, list),
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, obj.ToJson());

            return obj;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <param name="resCode"></param>
        /// <param name="resMsg"></param>
        /// <returns></returns>
        protected virtual BaseOutput Resp(ResCodeEnum resCode, string resMsg)
        {
            var res = new BaseOutput
            {
                ResCode = resCode,
                ResMsg = resMsg,
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            return res;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resCode"></param>
        /// <param name="resMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual DataOutput<T> Resp<T>(ResCodeEnum resCode, string resMsg, T data) where T : class, new()
        {
            var res = new DataOutput<T>()
            {
                ResCode = resCode,
                ResMsg = resMsg,
                Data = data,
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            return res;
        }

        /// <summary>
        /// 返回封装方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resCode"></param>
        /// <param name="resMsg"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual ListOutput<T> Resp<T>(ResCodeEnum resCode, string resMsg, List<T> list) where T : class, new()
        {
            var res = new ListOutput<T>()
            {
                ResCode = resCode,
                ResMsg = resMsg,
                Data = new ListViewModel<T>(list),
                TraceId = HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            return res;
        }
    }
}
