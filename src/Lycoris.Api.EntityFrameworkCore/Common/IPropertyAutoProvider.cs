using Lycoris.Api.Common.Snowflakes;
using Lycoris.Api.Model.Contexts;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lycoris.Api.EntityFrameworkCore.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyAutoProvider
    {
        /// <summary>
        /// 
        /// </summary>
        ISnowflakesMaker SnowflakesMaker { get; }

        /// <summary>
        /// 
        /// </summary>
        RequestContext? RequestContext { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void InsertIntercept(EntityEntry entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void UpdateIntercept(EntityEntry entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void DeleteIntercept(EntityEntry entities);
    }
}
