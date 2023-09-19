using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Transactions;

namespace Lycoris.Api.EntityFrameworkCore.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class MasterSlaveCommandInterceptor : DbCommandInterceptor
    {
        private readonly string[] SlaveConnectionString;

        /// <summary>
        /// 
        /// </summary>
        public MasterSlaveCommandInterceptor(params string[] SlaveConnectionString) => this.SlaveConnectionString = SlaveConnectionString ?? Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string GetSlaveConnectionString()
        {
            return SlaveConnectionString[new Random().Next(0, SlaveConnectionString.Length)] ?? throw new Exception("请配置读库");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        private void ChangeConnectToSlave(DbCommand command)
        {
            if (command.CommandText.ToLower().StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                if (command.Transaction == null && !(Transaction.Current != null && Transaction.Current.TransactionInformation.Status != TransactionStatus.Committed) && command.Connection != null)
                {
                    command.Connection.Close();
                    command.Connection.ConnectionString = GetSlaveConnectionString();
                    command.Connection.Open();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            this.ChangeConnectToSlave(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            this.ChangeConnectToSlave(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            this.ChangeConnectToSlave(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            this.ChangeConnectToSlave(command);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
