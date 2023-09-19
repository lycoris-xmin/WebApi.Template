using System.Data;

namespace Lycoris.Api.Core.Interceptors.Transactional
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string? ActionName = string.Empty;

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; } = null;
    }
}
