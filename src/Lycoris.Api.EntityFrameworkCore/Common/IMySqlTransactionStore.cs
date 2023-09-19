using Microsoft.EntityFrameworkCore.Storage;

namespace Lycoris.Api.EntityFrameworkCore.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMySqlTransactionStore
    {
        /// <summary>
        /// 当前正在执行的事务标识
        /// </summary>
        long? CurrentTransaction { get; set; }

        /// <summary>
        /// 事务是否已经启动
        /// </summary>
        bool IsStartingUow { get; set; }

        /// <summary>
        /// 当前正在执行的事务
        /// </summary>
        IDbContextTransaction? Transaction { get; set; }

        /// <summary>
        /// 事务是否为空
        /// </summary>
        bool IsNotNull { get; }
    }
}
