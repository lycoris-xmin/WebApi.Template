using Castle.DynamicProxy;
using Lycoris.Autofac.Extensions;

namespace Lycoris.Api.Core.Interceptors.Transactional
{
    /// <summary>
    /// 工作单元拦截器(AOP注入使用)
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Transient, IsInterceptor = true)]
    public class TransactionalInterceptor : IInterceptor
    {
        private readonly TransactionalAsyncInterceptor _AsyncInterceptor;

        public TransactionalInterceptor(TransactionalAsyncInterceptor AsyncInterceptor)
        {
            _AsyncInterceptor = AsyncInterceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            _AsyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}
