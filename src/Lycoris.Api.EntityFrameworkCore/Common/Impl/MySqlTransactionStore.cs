using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lycoris.Api.EntityFrameworkCore.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class MySqlTransactionStore : IMySqlTransactionStore
    {
        /// <summary>
        /// 
        /// </summary>
        public long? CurrentTransaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStartingUow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbContextTransaction? Transaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNotNull { get => this.IsStartingUow && this.Transaction != null && this.CurrentTransaction.HasValue && this.CurrentTransaction.Value > 0; }
    }
}
