using Lycoris.Api.Core.Logging;
using Lycoris.Api.Model.Cnstants;
using Lycoris.Api.Server.Shared;
using Lycoris.Base.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Lycoris.Api.Server.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpLoggingMiddleware : BaseMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="factory"></param>
        public HttpLoggingMiddleware(RequestDelegate next, ILycorisLoggerFactory factory) : base(next, factory.CreateLogger<HttpLoggingMiddleware>())
        {
            this.IgnoreOpptionsReuqest = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task InvokeHandlerAsync(HttpContext context)
        {
            // socket 直接过
            if (context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var traceId = context.Items.GetValue(HttpItems.TraceId);
            var httpMethod = context.Request.Method.ToLower();
            var requestIp = context.Items.GetValue(HttpItems.RequestIP);
            var userAgent = context.Items.GetValue(HttpItems.UserAgent);

            var requestHeaders = GetHttpReqeustHeadersMap(context);

            // 头部信息记录
            if (requestHeaders.Any())
                _logger.Info($"{httpMethod} -> request headers:[{string.Join(", ", requestHeaders.Select(x => $"{x.Key}:{x.Value}"))}]", traceId);

            var body = await GetHttpRequestBodyAsync(context);
            var path = httpMethod == "get" ? $"{(context.Request.Path.Value ?? "").TrimEnd('/')}{body ?? ""}" : $"{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "")}"; ;

            if (httpMethod == "get")
                _logger.Info($"{httpMethod} -> {path}");
            else
                _logger.Info($"{httpMethod} -> {path}{(body.IsNullOrEmpty() ? "" : $" -> body:{body}")}");

            await _next.Invoke(context);

            var responseHeaders = GetHttpResponseHeadersMap(context);

            var requestTime = context.Items.GetValue<DateTime>(HttpItems.RequestTime);
            var response = context.Items.GetValue(HttpItems.ResponseBody);
            var statusCode = context.Response.StatusCode;

            context.Response.OnCompleted(() => ResponseOnCompletedAsync(httpMethod, requestTime, responseHeaders, response, statusCode));

            context.Items.RemoveValue(HttpItems.ResponseBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetHttpReqeustHeadersMap(HttpContext context)
        {
            var headers = new Dictionary<string, string>()
            {
                { HttpHeaders.NginxRemoteIp , ""},
                { HttpHeaders.Host , ""},
                { HttpHeaders.Forwarded , ""},
                { HttpHeaders.UserAgent , ""},
                { HttpHeaders.Authentication , ""}
            };

            if (context.Request.Headers != null)
            {
                headers[HttpHeaders.NginxRemoteIp] = context.Request.Headers.ContainsKey(HttpHeaders.NginxRemoteIp) ? context.Request.Headers[HttpHeaders.NginxRemoteIp]! : "";

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.Host))
                    headers[HttpHeaders.Host] = context.Request.Headers[HttpHeaders.Host].ToString();

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.Forwarded))
                    headers[HttpHeaders.Forwarded] = context.Request.Headers[HttpHeaders.Forwarded]!;

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.UserAgent))
                    headers[HttpHeaders.UserAgent] = context.Request.Headers[HttpHeaders.UserAgent].ToString();

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.Authentication))
                    headers[HttpHeaders.Authentication] = context.Request.Headers[HttpHeaders.Authentication].ToString();
            }


            return headers.Where(x => !x.Value.IsNullOrEmpty()).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<string?> GetHttpRequestBodyAsync(HttpContext context)
        {
            if (context.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
                return context.Request.QueryString.Value ?? "";

            if (context.Request.Body == null)
                return "";

            if (context.Request.ContentType == null)
                return "";

            try
            {
                context.Request.EnableBuffering();

                var body = "";

                if (context.Request.Body.CanRead)
                {
                    body = await new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();

                    if (context.Request.ContentType.Contains("multipart/form-data"))
                        return await GetFormDataBodyAsync(context, body);

                    body = body.ToJsonString();
                }

                return body ?? "";
            }
            catch (Exception ex)
            {
                _logger.Error($"read request body exception", ex);
                return null;
            }
            finally
            {
                context.Request.Body?.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static async Task<string> GetFormDataBodyAsync(HttpContext context, string body)
        {
            var media = MediaTypeHeaderValue.Parse(context.Request.ContentType);
            var boundary = HeaderUtilities.RemoveQuotes(media.Boundary).Value;

            using var ms = new MemoryStream(Encoding.Default.GetBytes(body));
            var reader = new MultipartReader(boundary!, ms);
            var section = await reader.ReadNextSectionAsync();

            var result = "";

            while (section != null)
            {
                var propName = GetFormDataPropertyName(section.ContentDisposition);

                if (!propName.IsNullOrEmpty())
                {
                    result += $"\"{propName}\":";

                    if (!IsFileSection(section.ContentDisposition))
                    {
                        var propValue = await new StreamReader(section.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();
                        result += $"\"{propValue}\",";
                    }
                    else
                    {
                        result += $"\"file\":\"[File({GetFormDataFileName(section.ContentDisposition)})]\",";
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return result.IsNullOrEmpty() ? "" : $"{{{result.TrimEnd(',')}}}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetHttpResponseHeadersMap(HttpContext context)
        {
            if (context.Response.Headers == null)
                return new Dictionary<string, string>();

            var headers = new Dictionary<string, string>()
            {
                { "Content-Encoding", "" },
                { "ContentType", ""},
                { "StatusCode", ""},
                { "Redirect", ""}
            };

            headers["Content-Encoding"] = context.Response.Headers.ContainsKey("Content-Encoding") ? context.Response.Headers["Content-Encoding"].ToString() : "";
            headers["ContentType"] = context.Response.ContentType;
            headers["StatusCode"] = context.Response.StatusCode.ToString();

            if (context.Response.StatusCode == 302)
                headers["Redirect"] = context.Response.Headers.ContainsKey("Location") ? context.Response.Headers["Location"].ToString() : "";

            return headers.Where(x => !x.Value.IsNullOrEmpty()).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private static string? GetFormDataPropertyName(string? contentDisposition)
        {
            var strArray = contentDisposition?.Split(';');
            var formData = strArray?.FirstOrDefault(part => part.Contains("name="));
            return formData?.Split('=').Last().Trim('"') ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private static string? GetFormDataFileName(string? contentDisposition)
        {
            var strArray = contentDisposition?.Split(';');
            var formData = strArray?.FirstOrDefault(part => part.Contains("filename="));
            return formData?.Split('=').Last().Trim('"') ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private static bool IsFileSection(string? contentDisposition) => contentDisposition?.Split(';')?.FirstOrDefault(part => part.Contains("filename="))?.Any() ?? false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="requestTime"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private Task ResponseOnCompletedAsync(string httpMethod, DateTime requestTime, Dictionary<string, string> responseHeaders, string? response, int statusCode)
        {
            var log = new StringBuilder();
            var temp = responseHeaders.Select(x => $"{x.Key}: {x.Value}").ToArray();
            log.AppendFormat("{0} -> response headers:[{1}]", httpMethod, string.Join("; ", temp));
            _logger.Info(log.ToString());

            log.Clear();

            if (!response.IsNullOrEmpty())
            {
                log.AppendFormat("{0} -> response body", httpMethod);
                log.Append(" - ");
                log.Append(response);
            }
            else
                log.AppendFormat("{0} response", httpMethod);

            log.AppendFormat("- {0} - ", statusCode);

            var elapsedMilliseconds = (DateTime.Now - requestTime).TotalMilliseconds;

            log.AppendFormat("{0}ms", elapsedMilliseconds.ToString("0.000"));
            _logger.Info(log.ToString());

            return Task.CompletedTask;
        }
    }
}
