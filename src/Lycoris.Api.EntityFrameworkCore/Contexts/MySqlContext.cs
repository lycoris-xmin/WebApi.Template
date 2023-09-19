using Lycoris.Api.EntityFrameworkCore.Common;
using Lycoris.Api.EntityFrameworkCore.Common.Impl;
using Lycoris.Base.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace Lycoris.Api.EntityFrameworkCore.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        private IMySqlTransactionStore _transaction;

        /// <summary>
        /// 
        /// </summary>
        private IPropertyAutoProvider _provider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options"></param>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public MySqlContext([NotNull] DbContextOptions options) : base(options)
        {
            // 关闭DbContext默认事务
            Database.AutoTransactionsEnabled = false;
        }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 数据库工厂初始化数据库上下文
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        public void ServiceInitialization(IMySqlTransactionStore transaction, IPropertyAutoProvider provider)
        {
            _transaction = transaction;
            _provider = provider;
        }

        /// <summary>
        /// 数据库迁移，预热
        /// </summary>
        public Task WarmUpAsync()
        {
            // 迁移
            this.Database.Migrate();
            // 预热
            return this.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM  __efmigrationshistory");
        }

        /// <summary>
        /// 实体映射配置
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 设置数据库表的字符集
            builder.HasCharSet("utf8mb4");

            // 数据库表自动生成注册
            builder.TableAutoBuilder(this.GetType().Assembly);

            // 执行基类处理
            base.OnModelCreating(builder);
        }

        /// <summary>
        /// 重写 SaveChanges 方法
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().ToList();

            PropertyAutoProvider(entities);

            // 没有自动开启事务的情况下,保证主从表插入,主从表更新开启事务。
            var isManualTransaction = false;
            if (!Database.AutoTransactionsEnabled && _transaction != null && !_transaction.IsStartingUow && entities.Count > 1)
            {
                isManualTransaction = true;
                Database.AutoTransactionsEnabled = true;
            }

            var result = base.SaveChanges();

            // 如果手工开启了自动事务,用完后关闭
            if (isManualTransaction)
                Database.AutoTransactionsEnabled = false;

            return result;
        }

        /// <summary>
        /// 重写 SaveChanges 方法
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries().ToList();

            PropertyAutoProvider(entities);

            // 没有自动开启事务的情况下,保证主从表插入,主从表更新开启事务。
            var isManualTransaction = false;
            if (!Database.AutoTransactionsEnabled && _transaction != null && !_transaction.IsStartingUow && entities.Count > 1)
            {
                isManualTransaction = true;
                Database.AutoTransactionsEnabled = true;
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            // 如果手工开启了自动事务,用完后关闭。
            if (isManualTransaction)
                Database.AutoTransactionsEnabled = false;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        private void PropertyAutoProvider(List<EntityEntry> entities)
        {
            foreach (var item in entities)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        _provider.InsertIntercept(item);
                        break;
                    case EntityState.Modified:
                        _provider.UpdateIntercept(item);
                        break;
                    case EntityState.Deleted:
                        _provider.DeleteIntercept(item);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
