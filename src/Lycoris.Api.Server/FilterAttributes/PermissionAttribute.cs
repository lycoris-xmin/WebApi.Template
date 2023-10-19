using Lycoris.Api.Application.AppService.Permission;
using Lycoris.Api.Common.Extensions;
using Lycoris.Api.Model.Contexts;
using Lycoris.Api.Model.Exceptions;
using Lycoris.Api.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Lycoris.Api.Server.FilterAttributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : BaseActionAttribute
    {
        private readonly string[] Permissions;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="permissions"></param>
        public PermissionAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ActionHandlerBeforeAsync(ActionExecutingContext context)
        {
            // 没有配置允许的权限，默认全部禁止通过
            if (!this.Permissions.HasValue())
                throw new HttpStatusException(HttpStatusCode.Forbidden, "allowed permissions not configured");

            //
            var request = context.HttpContext.GetService<RequestContext>();

            // 获取权限
            var permission = context.HttpContext.GetService<IPermissionAppService>();

            request.User!.Permission = await permission.GetRolePermissionListAsync(request.User.RoleId) ?? Array.Empty<string>();

            // 没有满足的权限也返回403
            if (!request.User!.Permission.Any(x => this.Permissions.Contains(x)))
                throw new HttpStatusException(HttpStatusCode.Forbidden, "user does not have permission");
        }
    }
}
