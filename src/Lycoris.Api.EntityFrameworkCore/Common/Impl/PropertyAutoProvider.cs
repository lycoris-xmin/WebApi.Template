using Lycoris.Api.Common.Snowflakes;
using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.Model.Contexts;
using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Lycoris.Api.EntityFrameworkCore.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class PropertyAutoProvider : IPropertyAutoProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public ISnowflakesMaker SnowflakesMaker { get; }

        /// <summary>
        /// 请求上下文
        /// </summary>
        public RequestContext RequestContext { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SnowflakesMaker"></param>
        /// <param name="RequestContext"></param>
        public PropertyAutoProvider(ISnowflakesMaker SnowflakesMaker, RequestContext RequestContext)
        {
            this.SnowflakesMaker = SnowflakesMaker;
            this.RequestContext = RequestContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void InsertIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if ((item.Metadata.ClrType == typeof(long) || item.Metadata.ClrType == typeof(long?)) && item.Metadata.PropertyInfo != null && item.Metadata.PropertyInfo!.GetCustomAttribute<SnowflakeAttribute>(false) != null)
                {
                    if (item.CurrentValue != null && (long)item.CurrentValue > 0)
                        continue;

                    item.CurrentValue = this.SnowflakesMaker.GetSnowflakesId();
                }
                else if (item.Metadata.ClrType == typeof(DateTime))
                {
                    if (item.Metadata.Name.Equals("createtime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                            item.CurrentValue = DateTime.Now;
                    }
                }
                else if (this.RequestContext.User?.Id > 0)
                {
                    if (item.Metadata.Name.Equals("createuserid", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == this.RequestContext.User!.Id.GetType())
                    {
                        if (item.CurrentValue == null || (long)item.CurrentValue == 0)
                            item.CurrentValue = this.RequestContext.User?.Id ?? 0;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void UpdateIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if (item.Metadata.ClrType == typeof(DateTime))
                {
                    if (item.Metadata.Name.Equals("updatetime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                            item.CurrentValue = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void DeleteIntercept(EntityEntry entities)
        {

        }
    }
}
