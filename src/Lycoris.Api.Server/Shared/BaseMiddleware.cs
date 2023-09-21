using Lycoris.Api.Common;
using Lycoris.Api.Core.Logging;

namespace Lycoris.Api.Server.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        protected bool IgnoreOpptionsReuqest { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        protected bool IgnoreStaticFileReuqest { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        protected readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        protected readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        protected BaseMiddleware(RequestDelegate next, ILycorisLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (IgnoreOpptionsReuqest && IsOpptionsReuqest(context))
            {
                await _next.Invoke(context);
                return;
            }

            if (IgnoreStaticFileReuqest && IsStaticFileReuqest(context))
            {
                await _next.Invoke(context);
                return;
            }

            await InvokeHandlerAsync(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task InvokeHandlerAsync(HttpContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static bool IsSignalRRequest(HttpContext context) => context.Request.Headers.ContainsKey("Sec-WebSocket-Extensions") || context.Request.Headers.ContainsKey("Connection");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static bool IsOpptionsReuqest(HttpContext context) => context.Request.Method.Equals("OPTIONS", StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static bool IsStaticFileReuqest(HttpContext context) => context.Request.Path.HasValue && AppSettings.Path.StaticFilePath.Any(x => context.Request.Path.Value.StartsWith(x));
    }
}
