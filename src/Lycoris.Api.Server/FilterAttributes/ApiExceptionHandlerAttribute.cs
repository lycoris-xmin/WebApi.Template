using Lycoris.Api.Core.Logging;
using Lycoris.Api.Model.Cnstants;
using Lycoris.Api.Model.Exceptions;
using Lycoris.Api.Model.Global.Output;
using Lycoris.Base.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lycoris.Api.Server.FilterAttributes
{
    /// <summary>
    /// 接口全局异常捕获
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = false)]
    public class ApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public ApiExceptionHandlerAttribute(ILycorisLoggerFactory factory) => _logger = factory.CreateLogger<ApiExceptionHandlerAttribute>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            if (context.Exception is FriendlyException friendlyException)
                FriendlyExceptionHandler(context, friendlyException);
            else if (context.Exception is HttpStatusException httpStatusException)
                HttpStatusExceptionHanlder(context, httpStatusException);
            else
                OtherExceptionHanlder(context, context.Exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            if (context.Exception is FriendlyException friendlyException)
                FriendlyExceptionHandler(context, friendlyException);
            else if (context.Exception is HttpStatusException httpStatusException)
                HttpStatusExceptionHanlder(context, httpStatusException);
            else
                OtherExceptionHanlder(context, context.Exception);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void FriendlyExceptionHandler(ExceptionContext context, FriendlyException ex)
        {
            _logger.Warn($"handler friendly exception: {ex.LogMessage}");

            var res = new BaseOutput()
            {
                ResCode = ResCodeEnum.Friendly,
                ResMsg = ex.Message,
                TraceId = context.HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            context.Result = new JsonResult(res) { ContentType = "application/json", StatusCode = 200 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void HttpStatusExceptionHanlder(ExceptionContext context, HttpStatusException ex)
        {
            _logger.Warn($"handler httpstatus exception: {ex.Message}", ex);

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, "");

            context.Result = new ContentResult { Content = "", StatusCode = (int)ex.HttpStatusCode };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void OtherExceptionHanlder(ExceptionContext context, Exception ex)
        {
            _logger.Error($"hanlder exception:{ex.GetType().FullName}", ex);

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, ex.Message);

            context.Result = new ContentResult { Content = "", StatusCode = 500 };
        }
    }
}
