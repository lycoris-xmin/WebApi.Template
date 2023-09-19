using Lycoris.Api.Common.Snowflakes;
using Lycoris.Api.EntityFrameworkCore.Common;
using Lycoris.Api.EntityFrameworkCore.Contexts;
using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;


namespace Lycoris.Api.Core.EntityFrameworkCore.Impl
{
    /// <summary>
    /// Mysql事务
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class MySqlTransaction : IMySqlTransaction
    {
        /// <summary>
        /// Mysql数据库上下文
        /// </summary>
        private readonly MySqlContext _context;

        /// <summary>
        /// 事务状态
        /// </summary>
        private readonly IMySqlTransactionStore _transactionStore;

        /// <summary>
        /// 
        /// </summary>
        private readonly ISnowflakesMaker _snowflakesMaker;

        /// <summary>
        /// 事务状态
        /// </summary>
        public bool IsStartingUow { get => _transactionStore.IsStartingUow; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transactionStore"></param>
        /// <param name="snowflakesMaker"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MySqlTransaction(MySqlContext context, IMySqlTransactionStore transactionStore, ISnowflakesMaker snowflakesMaker)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _transactionStore = transactionStore;
            _snowflakesMaker = snowflakesMaker;
        }

        /// <summary>
        /// 开启数据库事务
        /// </summary>
        /// <param name="isolationLevel"></param>
        public long CreateTransaction(IsolationLevel? isolationLevel = null)
        {
            this._transactionStore.Transaction ??= GetDbContextTransaction(isolationLevel ?? IsolationLevel.RepeatableRead);
            var tmp = _snowflakesMaker.GetSnowflakesId();
            this._transactionStore.CurrentTransaction ??= tmp;
            return tmp;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="currentTransaction"></param>
        public void Commit(long currentTransaction)
        {
            if (this._transactionStore.IsNotNull && this._transactionStore.CurrentTransaction == currentTransaction)
            {
                this._transactionStore.Transaction!.Commit();
                _transactionStore.IsStartingUow = false;
            }
        }

        /// <summary>
        /// 事务提交(异步)
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CommitAsync(long currentTransaction, CancellationToken cancellationToken = default)
        {
            if (this._transactionStore.IsNotNull && this._transactionStore.CurrentTransaction == currentTransaction)
            {
                await this._transactionStore.Transaction!.CommitAsync(cancellationToken);
                _transactionStore.IsStartingUow = false;
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="currentTransaction"></param>
        public void Rollback(long currentTransaction)
        {
            if (this._transactionStore.IsNotNull && this._transactionStore.CurrentTransaction == currentTransaction)
            {
                this._transactionStore.Transaction!.Rollback();
                _transactionStore.IsStartingUow = false;
            }
        }

        /// <summary>
        /// 事务回滚(异步)
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RollbackAsync(long currentTransaction, CancellationToken cancellationToken = default)
        {
            if (this._transactionStore.IsNotNull && this._transactionStore.CurrentTransaction == currentTransaction)
            {
                await this._transactionStore.Transaction!.RollbackAsync(cancellationToken);
                _transactionStore.IsStartingUow = false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="currentTransaction"></param>
        public void Dispose(long currentTransaction)
        {
            if (this._transactionStore.IsNotNull && this._transactionStore.CurrentTransaction == currentTransaction)
            {
                this._transactionStore.Transaction!.Dispose();
                if (_transactionStore != null)
                    _transactionStore.IsStartingUow = false;
            }
        }

        /// <summary>
        /// 获取事务锁
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private IDbContextTransaction GetDbContextTransaction(IsolationLevel isolationLevel)
        {
            if (_transactionStore.IsStartingUow)
                throw new ArgumentException("transaction error");

            var trans = _context.Database.BeginTransaction(isolationLevel);

            _transactionStore.IsStartingUow = true;

            return trans;
        }
    }
}
