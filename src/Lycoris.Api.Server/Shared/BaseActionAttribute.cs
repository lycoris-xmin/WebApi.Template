using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lycoris.Api.Server.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseActionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await ActionHandlerBeforeAsync(context);
            await ActionHandlerAfterAsync(context, await next());
        }

        /// <summary>
        /// 接口执行前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task ActionHandlerBeforeAsync(ActionExecutingContext context) => Task.FromResult(true);

        /// <summary>
        /// 接口执行后
        /// </summary>
        /// <param name="context"></param>
        /// <param name="executedContext"></param>
        /// <returns></returns>
        public virtual async Task ActionHandlerAfterAsync(ActionExecutingContext context, ActionExecutedContext executedContext) => await Task.CompletedTask;

        /// <summary>
        /// 检测是否为允许匿名访问
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true-允许匿名访问 false-不允许匿名访问</returns>
        protected static bool CheckAllowAnonymous(ActionDescriptor context) => context.EndpointMetadata.Any(x => x is AllowAnonymousAttribute);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static bool CheckCustomeAttribute<T>(ActionDescriptor context) where T : Attribute => context.EndpointMetadata.Any(x => x is T);

        /// <summary>
        /// 未测试
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static T? GetCustomeAttribute<T>(ActionDescriptor context) where T : Attribute
        {
            var atr = context.EndpointMetadata.Where(x => x is T).FirstOrDefault();

            if (atr == null)
                return null;

            return atr is T _atr ? _atr : null;
        }
    }
}
