using Castle.DynamicProxy;
using System.Reflection;

namespace Lycoris.Api.Core.Interceptors.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncInterceptorHandler<T> : IAsyncInterceptor where T : Attribute
    {
        protected bool CheckAttribute { get; set; } = true;

        /// <summary>
        /// 同步拦截器
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptSynchronous(IInvocation invocation)
        {
            T? attribute = null;
            if (CheckAttribute)
            {
                attribute = GetAttribute(invocation);
                if (attribute == null)
                {
                    invocation.Proceed();
                    return;
                }
            }

            InterceptHandlger(invocation, attribute);
        }

        /// <summary>
        /// 异步拦截器无返回值
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            T? attribute = null;

            if (CheckAttribute)
            {
                attribute = GetAttribute(invocation);
                if (attribute == null)
                {
                    invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
                    return;
                }
            }

            invocation.ReturnValue = InterceptHandlgerAsync(invocation, attribute);
        }

        /// <summary>
        /// 异步拦截器-有返回值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            T? attribute = null;
            if (CheckAttribute)
            {
                attribute = GetAttribute(invocation);
                if (attribute == null)
                {
                    invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
                    return;
                }
            }

            invocation.ReturnValue = InterceptHandlgerAsync<TResult>(invocation, attribute);
        }

        /// <summary>
        /// 同步拦截器
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        public virtual void InterceptHandlger(IInvocation invocation, T? attribute) => invocation.Proceed();

        /// <summary>
        /// 异步拦截器
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        public virtual async Task InterceptHandlgerAsync(IInvocation invocation, T? attribute)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }

        /// <summary>
        /// 异步拦截器
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        public virtual async Task<TResult> InterceptHandlgerAsync<TResult>(IInvocation invocation, T? attribute)
        {
            TResult result;
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            result = await task;
            return result;
        }

        /// <summary>
        /// 获取拦截器对应的方法
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected virtual MethodInfo? GetCurrentMethod(IInvocation invocation)
        {
            if (invocation.Method.GetCustomAttribute<T>() != null)
                return invocation.Method;

            var implMethod = invocation.InvocationTarget
                                       .GetType()
                                       .GetMethods()
                                       .Where(m => m.Name == invocation.Method.Name && m.GetParameters().Length == invocation.Method.GetParameters().Length)
                                       .Where(x => x.GetCustomAttribute<T>(false) != null)
                                       .FirstOrDefault();

            return implMethod ?? invocation.Method;
        }

        /// <summary>
        /// 获取拦截器attrbute
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected virtual T? GetAttribute(IInvocation invocation) => GetCurrentMethod(invocation)?.GetCustomAttribute<T>();

        /// <summary>
        /// 获取当前方法的自定义attrbute
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected virtual TAttr? GetCustomeAttribute<TAttr>(IInvocation invocation) where TAttr : Attribute => GetCurrentMethod(invocation)?.GetCustomAttribute<TAttr>();

        /// <summary>
        /// 异步拦截器无事务处理-无返回值
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private static async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }

        /// <summary>
        /// 异步拦截器无事务处理-有返回值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private static async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            TResult result;
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            result = await task;
            return result;
        }
    }
}
