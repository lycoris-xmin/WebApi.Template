using Lycoris.Api.EntityFrameworkCore.Common;
using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Api.EntityFrameworkCore.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, Self = true)]
    public class MySqlContextScopedFactory : IDbContextFactory<MySqlContext>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IDbContextFactory<MySqlContext> _pooledFactory;

        /// <summary>
        /// 事务状态
        /// </summary>
        private readonly IMySqlTransactionStore _transaction;

        /// <summary>
        /// 
        /// </summary>
        private readonly IPropertyAutoProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pooledFactory"></param>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        public MySqlContextScopedFactory(IDbContextFactory<MySqlContext> pooledFactory, IMySqlTransactionStore transaction, IPropertyAutoProvider provider)
        {
            _pooledFactory = pooledFactory;
            _transaction = transaction;
            _provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MySqlContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            context.ServiceInitialization(_transaction, _provider);
            return context;
        }
    }
}
