using Castle.Core.Internal;
using Castle.DynamicProxy;
using Lycoris.Api.Core.EntityFrameworkCore;
using Lycoris.Api.Core.Interceptors.Base;
using Lycoris.Api.Core.Logging;
using Lycoris.Autofac.Extensions;

namespace Lycoris.Api.Core.Interceptors.Transactional
{
    /// <summary>
    /// 工作单元拦截器(实际业务逻辑)
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Transient, IsInterceptor = true)]
    public class TransactionalAsyncInterceptor : AsyncInterceptorHandler<TransactionalAttribute>
    {
        private readonly IMySqlTransaction _transaction;
        private readonly ILycorisLogger _logger;

        public TransactionalAsyncInterceptor(IMySqlTransaction transaction, ILycorisLoggerFactory factory)
        {
            _transaction = transaction;
            _logger = factory.CreateLogger<TransactionalAsyncInterceptor>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        public override void InterceptHandlger(IInvocation invocation, TransactionalAttribute? attribute)
        {
            var current = _transaction.CreateTransaction(attribute!.IsolationLevel);

            try
            {
                invocation.Proceed();
                _transaction.Commit(current);
            }
            catch (Exception ex)
            {
                ErrorLog(invocation, attribute, ex);
                _transaction.Rollback(current);
                throw;
            }
            finally
            {
                _transaction.Dispose(current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override async Task InterceptHandlgerAsync(IInvocation invocation, TransactionalAttribute? attribute)
        {
            var current = _transaction.CreateTransaction(attribute!.IsolationLevel);

            try
            {
                invocation.Proceed();
                var task = (Task)invocation.ReturnValue;
                await task;

                _transaction.Commit(current);
            }
            catch (Exception ex)
            {
                ErrorLog(invocation, attribute, ex);
                _transaction.Rollback(current);
                throw;
            }
            finally
            {
                _transaction.Dispose(current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override async Task<TResult> InterceptHandlgerAsync<TResult>(IInvocation invocation, TransactionalAttribute? attribute)
        {
            var current = _transaction.CreateTransaction(attribute!.IsolationLevel);

            try
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                var result = await task;
                _transaction.Commit(current);
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog(invocation, attribute, ex);
                _transaction.Rollback(current);
                throw;
            }
            finally
            {
                _transaction.Dispose(current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <param name="ex"></param>
        private void ErrorLog(IInvocation invocation, TransactionalAttribute attribute, Exception ex)
        {
            var methodInfo = invocation.Method ?? invocation.MethodInvocationTarget;
            var name = attribute.ActionName;
            if (name.IsNullOrEmpty())
                name = methodInfo.Name;

            _logger.Error($"{name} -> mysql transaction failed,handle exception:{ex.GetType().Name}", ex);
        }
    }
}

